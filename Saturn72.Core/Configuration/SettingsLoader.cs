using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using Saturn72.Core.Data;
using Saturn72.Extensions;

namespace Saturn72.Core.Configuration
{
    /// <summary>
    ///     Manager of data settings (connection string)
    /// </summary>
    public class SettingsLoader
    {
        protected const char COMMENT = '#';
        protected const char SEPARATOR = ':';
        protected const string FILE_NAME = "Settings.txt";

        #region ctor

        private SettingsLoader()
        {
        }

        #endregion

        /// <summary>
        ///     Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        protected virtual string MapPath(string path)
        {
            if (CommonHelper.IsWebApp())
                return HostingEnvironment.MapPath(path);

            //not hosted. For example, run in unit tests
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(baseDirectory, path);
        }

        /// <summary>
        ///     Parse settings
        /// </summary>
        /// <param name="text">Text of settings file</param>
        /// <returns>Parsed data settings</returns>
        protected virtual TSettings ParseSettings<TSettings>(string text) where TSettings : SettingsBase, new()
        {
            var tSettings = new TSettings();
            if (string.IsNullOrEmpty(text))
                return tSettings;

            //Old way of file reading. This leads to unexpected behavior when a user's FTP program transfers these files as ASCII (\r\n becomes \n).
            //var settings = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var settings = new List<string>();
            using (var reader = new StringReader(text))
            {
                string str;
                while ((str = reader.ReadLine()) != null)
                    settings.Add(str);
            }

            var propertyInfos = typeof (TSettings).GetProperties().Where(p=>p.GetSetMethod().NotNull());

            foreach (var setting in settings)
            {
                //const string connectionString = "DataConnectionString",
                //    dataprovider = "DataProvider",
                //    dbName = "DatabaseName",
                //    deviceHub = "DeviceHub";
                if (setting[0] == COMMENT)
                    continue;
                var separatorIndex = setting.IndexOf(SEPARATOR);
                if (separatorIndex == -1)
                    continue;

                var key = setting.Substring(0, separatorIndex).Trim();
                var value = setting.Substring(separatorIndex + 1).Trim();

                tSettings.RawSettings.Add(key, value);
                var pInfo = propertyInfos.FirstOrDefault(pi=>pi.Name.EqualsTo(key));

                if(pInfo.NotNull())
                    pInfo.SetValue(tSettings, value);

                //switch (key)
                //{
                //    case dbName:
                //        tSettings.DatabaseName = value;
                //        break;

                //    case dataprovider:
                //        tSettings.DataProvider = value;
                //        break;

                //    case connectionString:
                //        tSettings.DataConnectionString = value;
                //        break;
                //}
            }

            return tSettings;
        }

        /// <summary>
        ///     Convert data settings to string representation
        /// </summary>
        /// <param name="settings">Settings</param>
        /// <returns>Text</returns>
        protected virtual string ComposeSettings(DataSettings settings)
        {
            if (settings == null)
                return "";

            return string.Format("DataProvider: {0}{2}DataConnectionString: {1}{2}",
                settings.DataProvider,
                settings.DataConnectionString,
                Environment.NewLine
                );
        }

        public string GetSettingContent(string filePath = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                //use webHelper.MapPath instead of HostingEnvironment.MapPath which is not available in unit tests
                filePath = Path.Combine(MapPath("~/App_Data/"), FILE_NAME);
            }
            return File.Exists(filePath)
                ? File.ReadAllText(filePath)
                : null;
        }

        /// <summary>
        ///     Load settings
        /// </summary>
        /// <param name="filePath">File path; pass null to use default settings file path</param>
        /// <returns></returns>
        public static TSettings LoadSettings<TSettings>(string filePath = null) where TSettings:SettingsBase, new()
        {
            var loader = new SettingsLoader();
            var settingContent = loader.GetSettingContent(filePath);
            return settingContent != null
                ? loader.ParseSettings<TSettings>(settingContent)
                : new TSettings();
        }

        /// <summary>
        ///     Save settings to a file
        /// </summary>
        /// <param name="settings"></param>
        public virtual void SaveSettings(DataSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            //use webHelper.MapPath instead of HostingEnvironment.MapPath which is not available in unit tests
            var filePath = Path.Combine(MapPath("~/App_Data/"), FILE_NAME);
            if (!File.Exists(filePath))
            {
                using (File.Create(filePath))
                {
                    //we use 'using' to close the file after it's created
                }
            }

            var text = ComposeSettings(settings);
            File.WriteAllText(filePath, text);
        }
    }
}