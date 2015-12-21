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
using Saturn72.Core.ComponentModel;
using Saturn72.Core.Plugins;
using Saturn72.Extensions;

[assembly: PreApplicationStartMethod(typeof(PluginManager), "Initialize")]
namespace Saturn72.Core.Plugins
{
    /// <summary>
    ///     Sets the application up for the plugin referencing
    /// </summary>
    public class PluginManager
    {
        #region Const

        private static string _installedPluginsFilePath;

        private static string InstalledPluginsFilePath => _installedPluginsFilePath ??
                                                          (_installedPluginsFilePath = BuildInstalledPluginFilePath());

        private static string BuildInstalledPluginFilePath()
        {
            var relativePath = (CommonHelper.IsWebApp() ? "~/App_Data/" : string.Empty) + "InstalledPlugins.txt";
            return CommonHelper.BuildRelativePath(relativePath);
        }

        #endregion

        #region Fields

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();
        private static DirectoryInfo _shadowCopyFolder;

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
            InitializeWithParameters(pluginsSettings.PluginFolder,
                pluginsSettings.ShadowCopyFolder,
                pluginsSettings.ClearShadowDirectoryOnStartup);
        }

        public static void InitializeWithParameters(string pluginFolderName, string shadowCopyFolder,
            bool clearShadowDirectoryOnStartup)
        {
            using (new WriteLockDisposable(Locker))
            {
                // TODO: Add verbose exception handling / raising here since this is happening on app startup and could
                // prevent app from starting altogether


                _shadowCopyFolder = new DirectoryInfo(shadowCopyFolder);
                if (!_shadowCopyFolder.Exists)
                    Trace.WriteLine("Could not find plugins folder in path: " + _shadowCopyFolder);

                var referencedPlugins = new List<PluginDescriptor>();
                var incompatiblePlugins = new List<string>();
                try
                {
                    var installedPluginSystemNames =
                        PluginFileParser.ParseInstalledPluginsFile(InstalledPluginsFilePath);

                    //ensure folders are created
                    Debug.WriteLine("Creating plugin folder (if not already exists). Folder path: " +
                                    pluginFolderName);

                    Directory.CreateDirectory(pluginFolderName);

                    Debug.WriteLine(
                        "Creating shadow copy folder (if not already exists) and querying for dlls. Folder path: " +
                        _shadowCopyFolder);
                    _shadowCopyFolder.Create();

                    //get list of all files in bin
                    var binFiles = _shadowCopyFolder
                        .GetFiles("*", SearchOption.AllDirectories);
                    if (clearShadowDirectoryOnStartup)
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
                    foreach (var dfd in GetDescriptionFilesAndDescriptors(pluginFolderName))
                    {
                        var descriptionFile = dfd.Key;
                        var pluginDescriptor = dfd.Value;
                        Debug.WriteLine("Start importing {0} plugin from {1}", pluginDescriptor.FriendlyName,
                            pluginDescriptor.SystemName);

                        //ensure that version of plugin is valid
                        if (
                            !pluginDescriptor.SupportedVersions.Contains(Saturn72ServerVersion.CurrentVersion,
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
            Guard.MustFollow(pluginFolderName.HasValue(), () => { throw new ArgumentNullException("pluginFolderName"); });
            var pluginFolder = new DirectoryInfo(pluginFolderName);
            //create list (<file info, parsed plugin descritor>)
            var result = new List<KeyValuePair<FileInfo, PluginDescriptor>>();
            //add display order and path to list
            foreach (var descriptionFile in pluginFolder.GetFiles("Description.txt", SearchOption.AllDirectories))
            {
                var pluginDescriptor = PluginFileParser.ParsePluginDescriptionFile(descriptionFile.FullName);
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
                if (!fileNameWithoutExt.HasValue())
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
            var shadowCopiedPluginPath = GetDeploymentPathInfo(plugin);
            //we can now register the plugin definition
            var shadowCopiedAssembly = Assembly.Load(AssemblyName.GetAssemblyName(shadowCopiedPluginPath.FullName));

            //add the reference to the build manager
            Debug.WriteLine("Adding to BuildManager: '{0}'", shadowCopiedAssembly.FullName);

            if (CommonHelper.IsWebApp())
                BuildManager.AddReferencedAssembly(shadowCopiedAssembly);

            return shadowCopiedAssembly;
        }

        private static FileInfo GetDeploymentPathInfo(FileInfo plugin)
        {
            if (CommonHelper.IsWebApp() && NetCommonHelper.GetTrustLevel() == AspNetHostingPermissionLevel.Unrestricted)
            {
                var directory = AppDomain.CurrentDomain.DynamicDirectory;
                Debug.WriteLine(plugin.FullName + " to " + directory);

                //were running in full trust so copy to standard dynamic folder
                return GetFullTrustDeploymentPath(plugin, new DirectoryInfo(directory));
            }

            return GetMediumTrustDeploymentPath(plugin, _shadowCopyFolder);
        }

        private static FileInfo GetMediumTrustDeploymentPath(FileSystemInfo plugin, DirectoryInfo shadowCopyPlugFolder)
        {
            var shouldCopy = true;
            var shadowCopiedPlug = new FileInfo(Path.Combine(shadowCopyPlugFolder.FullName, plugin.Name));

            //check if a shadow copied file already exists and if it does, check if it's updated, if not don't copy
            if (shadowCopiedPlug.Exists)
            {
                //it's better to use LastWriteTimeUTC, but not all file systems have this property
                //maybe it is better to compare file hash?
                var areFilesIdentical = shadowCopiedPlug.CreationTimeUtc.Ticks >= plugin.CreationTimeUtc.Ticks;
                if (areFilesIdentical)
                {
                    Debug.WriteLine("Not copying; files appear identical: '{0}'", shadowCopiedPlug.Name);
                    shouldCopy = false;
                }
                else
                {
                    //delete an existing file

                    //More info: http://www.nopcommerce.com/boards/t/11511/access-error-nopplugindiscountrulesbillingcountrydll.aspx?p=4#60838
                    Debug.WriteLine("New plugin found; Deleting the old file: '{0}'", shadowCopiedPlug.Name);
                    File.Delete(shadowCopiedPlug.FullName);
                }
            }

            if (shouldCopy)
            {
                try
                {
                    File.Copy(plugin.FullName, shadowCopiedPlug.FullName, true);
                }
                catch (IOException)
                {
                    Debug.WriteLine(shadowCopiedPlug.FullName + " is locked, attempting to rename");
                    //this occurs when the files are locked,
                    //for some reason devenv locks plugin files some times and for another crazy reason you are allowed to rename them
                    //which releases the lock, so that it what we are doing here, once it's renamed, we can re-shadow copy
                    try
                    {
                        var oldFile = shadowCopiedPlug.FullName + Guid.NewGuid().ToString("N") + ".old";
                        File.Move(shadowCopiedPlug.FullName, oldFile);
                    }
                    catch (IOException exc)
                    {
                        throw new IOException(shadowCopiedPlug.FullName + " rename failed, cannot initialize plugin",
                            exc);
                    }
                    //ok, we've made it this far, now retry the shadow copy
                    File.Copy(plugin.FullName, shadowCopiedPlug.FullName, true);
                }
            }

            return shadowCopiedPlug;
        }

        private static FileInfo GetFullTrustDeploymentPath(FileSystemInfo plug, DirectoryInfo shadowCopyPlugFolder)
        {
            var shadowCopiedPlug = new FileInfo(Path.Combine(shadowCopyPlugFolder.FullName, plug.Name));
            try
            {
                File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
            }
            catch (IOException)
            {
                Debug.WriteLine(shadowCopiedPlug.FullName + " is locked, attempting to rename");
                //this occurs when the files are locked,
                //for some reason devenv locks plugin files some times and for another crazy reason you are allowed to rename them
                //which releases the lock, so that it what we are doing here, once it's renamed, we can re-shadow copy
                try
                {
                    var oldFile = shadowCopiedPlug.FullName + Guid.NewGuid().ToString("N") + ".old";
                    File.Move(shadowCopiedPlug.FullName, oldFile);
                }
                catch (IOException exc)
                {
                    throw new IOException(shadowCopiedPlug.FullName + " rename failed, cannot initialize plugin", exc);
                }
                //ok, we've made it this far, now retry the shadow copy
                File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
            }
            return shadowCopiedPlug;
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
        #endregion
    }
}