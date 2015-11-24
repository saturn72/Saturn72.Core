namespace Automation.Core.Data.Plugins
{
    public abstract class BaseDataProviderManager
    {
        private DataSettings _dataSettings;

        public DataSettings DataSettings
        {
            get { return _dataSettings ?? (_dataSettings = LoadDataSettings()); }
        }

        protected virtual DataSettings LoadDataSettings()
        {
            var manager = new DataSettingsLoader();
            return manager.LoadSettings();
        }

        public abstract IDataProvider LoadDataProvider();
    }
}