using Saturn72.Core.Configuration;

namespace Saturn72.Core.Data
{
    public abstract class BaseDataProviderManager
    {
        private BaseDataProvider _dataProvider;
        private DataSettings _dataSettings;

        public DataSettings DataSettings
        {
            get { return _dataSettings ?? (_dataSettings = SettingsLoader.LoadSettings<DataSettings>()); }
        }

        public BaseDataProvider DataProvider
        {
            get { return _dataProvider ?? (_dataProvider = LoadDataProvider()); }
        }

        protected abstract BaseDataProvider LoadDataProvider();
    }
}