﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Compilation;
using Saturn72.Core.ComponentModel;
using Saturn72.Core.Plugins;
using Saturn72.Extensions;

namespace Saturn72.Core.Infrastructure
{
    /// <summary>
    ///     Represents AppDomainLoader.
    ///     This object in charge to load all dynamic assemblies into domain (usually on startup)
    /// </summary>
    public class AppDomainLoader
    {
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();
        private static string _shadowCopyDirectory;

        /// <summary>
        /// Loads all components to AppDomain
        /// </summary>
        /// <param name="data">AppDomainLoadData <see cref="AppDomainLoadData"/></param>
        public static void Load(AppDomainLoadData data)
        {
            var componentDirectories = new[] {data.PluginsParentDirecotry, data.ModulesParentDirecotry};

            foreach (var componentDir in componentDirectories)
            {
                if (!componentDir.HasValue())
                    continue;

                using (new WriteLockDisposable(Locker))
                {
                    _shadowCopyDirectory = componentDir + @"\bin";
                    var binFiles = Directory.Exists(_shadowCopyDirectory)
                        ? Directory.GetFiles(_shadowCopyDirectory)
                        : new string[] {};

                    if (data.DeleteShadowDirectoryOnStartup && Directory.Exists(_shadowCopyDirectory))
                    {
                        foreach (var f in binFiles)
                        {
                            Debug.WriteLine("Deleting " + f);
                            try
                            {
                                IoUtil.DeleteFile(f);
                                }
                            catch (Exception exc)
                            {
                                Debug.WriteLine("Error deleting file " + f + ". Exception: " + exc);
                            }
                        }
                        IoUtil.DeleteDirectory(_shadowCopyDirectory);
                    }

                    IoUtil.CreateDirectoryIfNotExists(_shadowCopyDirectory);
                    try
                    {
                        var dynamicLoadedFiles = Directory.GetFiles(componentDir, "*.dll", SearchOption.AllDirectories)
                            //Not in the shadow copy
                            .Where(x => !binFiles.Contains(x))
                            .ToArray();

                        //load all other referenced assemblies now
                        foreach (var dlf in dynamicLoadedFiles
                            .Where(x => !IsAlreadyLoaded(x)))
                        {
                            Guard.NotEmpty(dlf);
                            PerformFileDeploy(new FileInfo(dlf));
                        }
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        var msg = string.Empty;
                        foreach (var exception in ex.LoaderExceptions)
                            msg = msg + (exception.Message + Environment.NewLine);

                        var fail = new Exception(msg, ex);
                        Debug.WriteLine(fail.Message, fail);

                        throw fail;
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
                }
            }
        }

        private static bool IsAlreadyLoaded(string file)
        {
            //compare full assembly name
            //var fileAssemblyName = AssemblyName.GetAssemblyName(fileInfo.FullName);
            //foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    if (a.FullName.Equals(fileAssemblyName.Type, StringComparison.InvariantCultureIgnoreCase))
            //        return true;
            //}
            //return false;

            //do not compare the full assembly name, just filename
            try
            {
                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);
                if (!fileNameWithoutExt.HasValue())
                    throw new Exception(string.Format("Cannot get file extnension for {0}", file));
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
        /// <param name="component">Plugin file info</param>
        /// <returns>Assembly</returns>
        private static Assembly PerformFileDeploy(FileInfo component)
        {
            VerifyNotNullComponent(component);
            var shadowCopiedPluginPath = GetDeploymentPathInfo(component);
            //we can now register the plugin definition
            var shadowCopiedAssembly = Assembly.Load(AssemblyName.GetAssemblyName(shadowCopiedPluginPath.FullName));

            //add the reference to the build manager
            Debug.WriteLine("Adding to BuildManager: '{0}'", shadowCopiedAssembly.FullName);

            if (CommonHelper.IsWebApp())
                BuildManager.AddReferencedAssembly(shadowCopiedAssembly);

            return shadowCopiedAssembly;
        }

        private static void VerifyNotNullComponent(FileInfo componentFileInfo)
        {
            Guard.NotNull(componentFileInfo);
            Guard.NotNull(componentFileInfo.Directory);
            Guard.NotNull(componentFileInfo.Directory.Parent, () =>
            {
                var message = "The component directory for the {0} file exists in a folder outside of the allowed saturn72 folder heirarchy"
                    .AsFormat(componentFileInfo.Name);
                throw new InvalidOperationException(message);
            });
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

            return GetMediumTrustDeploymentPath(plugin, _shadowCopyDirectory);
        }

        private static FileInfo GetMediumTrustDeploymentPath(FileSystemInfo component, string shadowCopyDirPath)
        {
            var shouldCopy = true;
            var shadowCopiedPlug = new FileInfo(Path.Combine(shadowCopyDirPath, component.Name));

            //check if a shadow copied file already exists and if it does, check if it's updated, if not don't copy
            if (shadowCopiedPlug.Exists)
            {
                //it's better to use LastWriteTimeUTC, but not all file systems have this property
                //maybe it is better to compare file hash?
                var areFilesIdentical = shadowCopiedPlug.CreationTimeUtc.Ticks >= component.CreationTimeUtc.Ticks;
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
                    File.Copy(component.FullName, shadowCopiedPlug.FullName, true);
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
                    File.Copy(component.FullName, shadowCopiedPlug.FullName, true);
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



    }
}