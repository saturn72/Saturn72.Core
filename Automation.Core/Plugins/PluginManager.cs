using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;
using Automation.Core.ComponentModel;
using Automation.Core.Plugins;
using Automation.Extensions;

[assembly: PreApplicationStartMethod(typeof (PluginManager), "Initialize")]
namespace Automation.Core.Plugins
{
    /// <summary>
    ///     Sets the application up for the plugin referencing
    /// </summary>
    public class PluginManager
    {
        #region Const

        private static string _installedPluginsFilePath;

        private static string InstalledPluginsFilePath
        {
            get
            {
                return _installedPluginsFilePath ??
                       (_installedPluginsFilePath = BuildInstalledPluginFilePath());
            }
        }

        private static string BuildInstalledPluginFilePath()
        {
            var relativePath = (CommonHelper.IsWebApp() ? "~/App_Data/" : string.Empty) + "InstalledPlugins.txt";
            return CommonHelper.BuildRelativePath(relativePath);
        }

        #endregion

        #region Fields

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();
        private static string _shadowCopyFolder;

        #endregion

        #region Methods

        /// <summary>
        ///     Returns a collection of all referenced plugin assemblies that have been shadow copied
        /// </summary>
        public static IEnumerable<PluginDescriptor> ReferencedPlugins { get; set; }

        /// <summary>
        ///     Returns a collection of all plugin which are not compatible with the current version
        /// </summary>
        public static IEnumerable<string> IncompatiblePlugins { get; set; }

        /// <summary>
        ///     Initialize
        /// </summary>
        public static void Initialize()
        {
            var pluginsSettings = PluginSettings.LoadSettings();
            using (new WriteLockDisposable(Locker))
            {
                // TODO: Add verbose exception handling / raising here since this is happening on app startup and could
                // prevent app from starting altogether
                _shadowCopyFolder = pluginsSettings.ShadowCopyFolder;
                if (!Directory.Exists(_shadowCopyFolder))
                    Trace.WriteLine("Could not find plugins folder in path: " + _shadowCopyFolder);

                var referencedPlugins = new List<PluginDescriptor>();
                var incompatiblePlugins = new List<string>();
                try
                {
                    var installedPluginSystemNames =
                        PluginFileParser.ParseInstalledPluginsFile(InstalledPluginsFilePath);

                    //ensure folders are created
                    Debug.WriteLine("Creating plugin folder (if not already exists). Folder path: " +
                                    pluginsSettings.PluginFolder);

                    Directory.CreateDirectory(pluginsSettings.PluginFolder);

                    Debug.WriteLine(
                        "Creating shadow copy folder (if not already exists) and querying for dlls. Folder path: " +
                        _shadowCopyFolder);
                    Directory.CreateDirectory(_shadowCopyFolder);

                    //get list of all files in bin
                    var binFiles = IoHelper.GetDirectoryInfo(_shadowCopyFolder)
                        .GetFiles("*", SearchOption.AllDirectories);
                    if (pluginsSettings.ClearShadowDirectoryOnStartup)
                    {
                        //clear out shadow copied plugins
                        foreach (var f in binFiles)
                        {
                            Debug.WriteLine("Deleting " + f.Name);
                            try
                            {
                                File.Delete(f.FullName);
                            }
                            catch (Exception exc)
                            {
                                Debug.WriteLine("Error deleting file " + f.Name + ". Exception: " + exc);
                            }
                        }
                    }

                    //load description files
                    foreach (var dfd in GetDescriptionFilesAndDescriptors(pluginsSettings.PluginFolder))
                    {
                        var descriptionFile = dfd.Key;
                        var pluginDescriptor = dfd.Value;
                        Debug.WriteLine("Start importing {0} plugin from {1}", pluginDescriptor.FriendlyName,
                            pluginDescriptor.SystemName);

                        //ensure that version of plugin is valid
                        if (
                            !pluginDescriptor.SupportedVersions.Contains(AutomationServerVersion.CurrentVersion,
                                StringComparer.InvariantCultureIgnoreCase))
                        {
                            incompatiblePlugins.Add(pluginDescriptor.SystemName);
                            continue;
                        }

                        //some validation
                        if (string.IsNullOrWhiteSpace(pluginDescriptor.SystemName))
                            throw new Exception(
                                string.Format(
                                    "A plugin '{0}' has no system name. Try assigning the plugin a unique name and recompiling.",
                                    descriptionFile.FullName));
                        if (referencedPlugins.Contains(pluginDescriptor))
                            throw new Exception(string.Format("A plugin with '{0}' system name is already defined",
                                pluginDescriptor.SystemName));

                        //set 'Installed' property
                        pluginDescriptor.Installed = installedPluginSystemNames
                            .FirstOrDefault(
                                x => x.Equals(pluginDescriptor.SystemName, StringComparison.InvariantCultureIgnoreCase)) !=
                                                     null;

                        try
                        {
                            if (descriptionFile.Directory == null)
                                throw new Exception(
                                    string.Format("Directory cannot be resolved for '{0}' description file",
                                        descriptionFile.Name));
                            //get list of all DLLs in plugins (not in bin!)
                            var pluginFiles = descriptionFile.Directory.GetFiles("*.dll", SearchOption.AllDirectories)
                                //just make sure we're not registering shadow copied plugins
                                .Where(x => !binFiles.Select(q => q.FullName).Contains(x.FullName))
                                .Where(x => IsPackagePluginFolder(x.Directory))
                                .ToList();

                            //other plugin description info
                            var mainPluginFile = pluginFiles
                                .FirstOrDefault(
                                    x =>
                                        x.Name.Equals(pluginDescriptor.PluginFileName,
                                            StringComparison.InvariantCultureIgnoreCase));
                            pluginDescriptor.OriginalAssemblyFile = mainPluginFile;

                            //shadow copy main plugin file
                            pluginDescriptor.ReferencedAssembly = PerformFileDeploy(mainPluginFile);

                            //load all other referenced assemblies now
                            foreach (var plugin in pluginFiles
                                .Where(
                                    x =>
                                        !x.Name.Equals(mainPluginFile.Name, StringComparison.InvariantCultureIgnoreCase))
                                .Where(x => !IsAlreadyLoaded(x)))
                                PerformFileDeploy(plugin);

                            //init plugin type (only one plugin per assembly is allowed)
                            foreach (var t in pluginDescriptor.ReferencedAssembly.GetTypes())
                                if (typeof (IInstallablePlugin).IsAssignableFrom(t))
                                    if (!t.IsInterface)
                                        if (t.IsClass && !t.IsAbstract)
                                        {
                                            pluginDescriptor.PluginType = t;
                                            break;
                                        }

                            referencedPlugins.Add(pluginDescriptor);
                        }
                        catch (ReflectionTypeLoadException ex)
                        {
                            var msg = string.Empty;
                            foreach (var e in ex.LoaderExceptions)
                                msg += e.Message + Environment.NewLine;

                            var fail = new Exception(msg, ex);
                            Debug.WriteLine(fail.Message, fail);

                            throw fail;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var msg = string.Empty;
                    for (var e = ex; e != null; e = e.InnerException)
                        msg += e.Message + Environment.NewLine;

                    var fail = new Exception(msg, ex);
                    Debug.WriteLine(fail.Message, fail);

                    throw fail;
                }

                ReferencedPlugins = referencedPlugins;
                IncompatiblePlugins = incompatiblePlugins;
            }
        }


        /// <summary>
        ///     Mark plugin as installed
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        public static void MarkPluginAsInstalled(string systemName)
        {
            if (string.IsNullOrEmpty(systemName))
                throw new ArgumentNullException("systemName");

            if (!File.Exists(InstalledPluginsFilePath))
                using (File.Create(InstalledPluginsFilePath))
                {
                    //we use 'using' to close the file after it's created
                }


            var installedPluginSystemNames = PluginFileParser.ParseInstalledPluginsFile(InstalledPluginsFilePath);
            var alreadyMarkedAsInstalled = installedPluginSystemNames
                .FirstOrDefault(x => x.Equals(systemName, StringComparison.InvariantCultureIgnoreCase)) != null;
            if (!alreadyMarkedAsInstalled)
                installedPluginSystemNames.Add(systemName);
            PluginFileParser.SaveInstalledPluginsFile(installedPluginSystemNames, InstalledPluginsFilePath);
        }

        /// <summary>
        ///     Mark plugin as uninstalled
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        public static void MarkPluginAsUninstalled(string systemName)
        {
            if (string.IsNullOrEmpty(systemName))
                throw new ArgumentNullException("systemName");

            var filePath = HostingEnvironment.MapPath(InstalledPluginsFilePath);
            if (!File.Exists(filePath))
                using (File.Create(filePath))
                {
                    //we use 'using' to close the file after it's created
                }


            var installedPluginSystemNames = PluginFileParser.ParseInstalledPluginsFile(InstalledPluginsFilePath);
            var alreadyMarkedAsInstalled = installedPluginSystemNames
                .FirstOrDefault(x => x.Equals(systemName, StringComparison.InvariantCultureIgnoreCase)) != null;
            if (alreadyMarkedAsInstalled)
                installedPluginSystemNames.Remove(systemName);
            PluginFileParser.SaveInstalledPluginsFile(installedPluginSystemNames, filePath);
        }

        /// <summary>
        ///     Mark plugin as uninstalled
        /// </summary>
        public static void MarkAllPluginsAsUninstalled()
        {
            var filePath = HostingEnvironment.MapPath(InstalledPluginsFilePath);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        #endregion

        #region Utilities

        /// <summary>
        ///     Get description files
        /// </summary>
        /// <param name="pluginFolder">Plugin direcotry info</param>
        /// <returns>Original and parsed description files</returns>
        private static IEnumerable<KeyValuePair<FileInfo, PluginDescriptor>> GetDescriptionFilesAndDescriptors(
            string pluginFolderName)
        {
            Guard.MustFollow(pluginFolderName.HaveValue(), () => { throw new ArgumentNullException("pluginFolderName"); });
            var pluginFolder = new DirectoryInfo(pluginFolderName);
            //create list (<file info, parsed plugin descritor>)
            var result = new List<KeyValuePair<FileInfo, PluginDescriptor>>();
            //add display order and path to list
            foreach (var descriptionFile in pluginFolder.GetFiles("Description.txt", SearchOption.AllDirectories))
            {
                if (!IsPackagePluginFolder(descriptionFile.Directory))
                    continue;

                //parse file
                var pluginDescriptor = PluginFileParser.ParsePluginDescriptionFile(descriptionFile.FullName);

                //populate list
                result.Add(new KeyValuePair<FileInfo, PluginDescriptor>(descriptionFile, pluginDescriptor));
            }

            //sort list by display order. NOTE: Lowest DisplayOrder will be first i.e 0 , 1, 1, 1, 5, 10
            //it's required: http://www.nopcommerce.com/boards/t/17455/load-plugins-based-on-their-displayorder-on-startup.aspx
            result.Sort((firstPair, nextPair) => firstPair.Value.DisplayOrder.CompareTo(nextPair.Value.DisplayOrder));
            return result;
        }

        /// <summary>
        ///     Indicates whether assembly file is already loaded
        /// </summary>
        /// <param name="fileInfo">File info</param>
        /// <returns>ActualValue</returns>
        private static bool IsAlreadyLoaded(FileInfo fileInfo)
        {
            //compare full assembly name
            //var fileAssemblyName = AssemblyName.GetAssemblyName(fileInfo.FullName);
            //foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    if (a.FullName.Equals(fileAssemblyName.FullName, StringComparison.InvariantCultureIgnoreCase))
            //        return true;
            //}
            //return false;

            //do not compare the full assembly name, just filename
            try
            {
                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                if (!fileNameWithoutExt.HaveValue())
                    throw new Exception(string.Format("Cannot get file extnension for {0}", fileInfo.Name));
                foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var assemblyName = a.FullName.Split(',').FirstOrDefault();
                    if (fileNameWithoutExt.Equals(assemblyName, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Cannot validate whether an assembly is already loaded. " + exc);
            }
            return false;
        }

        /// <summary>
        ///     Perform file deply
        /// </summary>
        /// <param name="plugin">Plugin file info</param>
        /// <returns>Assembly</returns>
        private static Assembly PerformFileDeploy(FileInfo plugin)
        {
            VerifyNotNullPlugin(plugin);

            var isWebApp = CommonHelper.IsWebApp();
            var shadowCopiedPlug = isWebApp
                ? AspNetExt.GetAspNetDeploymentPath(plugin, _shadowCopyFolder)
                : GetDeploymentPath(plugin);

            //we can now register the plugin definition
            var shadowCopiedAssembly = Assembly.Load(AssemblyName.GetAssemblyName(shadowCopiedPlug.FullName));

            //add the reference to the build manager
            Debug.WriteLine("Adding to BuildManager: '{0}'", shadowCopiedAssembly.FullName);

            if (isWebApp)
                BuildManager.AddReferencedAssembly(shadowCopiedAssembly);

            return shadowCopiedAssembly;
        }

        private static void VerifyNotNullPlugin(FileInfo plug)
        {
            Guard.NotNull(plug);
            Guard.NotNull(plug.Directory);
            Guard.NotNull(plug.Directory.Parent, () =>
            {
                var message = "The plugin directory for the {0} file exists in a folder outside of the allowed saturn72 folder heirarchy"
                    .AsFormat(plug.Name);
                throw new InvalidOperationException(message);
            });
        }

        private static FileInfo GetDeploymentPath(FileInfo plug)
        {
            var shadowCopyPlugFolder = Directory.CreateDirectory(_shadowCopyFolder);
            return AspNetExt.InitializeMediumTrust(plug, shadowCopyPlugFolder);
        }


        /// <summary>
        ///     Determines if the folder is a bin plugin folder for a package
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private static bool IsPackagePluginFolder(DirectoryInfo folder)
        {
            if (folder == null) return false;
            if (folder.Parent == null) return false;
            if (!folder.Parent.Name.Equals("Plugins", StringComparison.InvariantCultureIgnoreCase)) return false;
            return true;
        }

        #endregion
    }
}