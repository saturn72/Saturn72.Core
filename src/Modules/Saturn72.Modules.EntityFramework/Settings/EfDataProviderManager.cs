using Saturn72.Core;
using Saturn72.Core.Configuration;
using Saturn72.Extensions;

namespace Saturn72.Modules.EntityFramework.Settings
{
    public class EfDataProviderManager
    {
        private IDatabaseProvider _dataProvider;
        private DataSettings _dataSettings;

        public DataSettings DataSettings
        {
            get { return _dataSettings ?? (_dataSettings = SettingsLoader.LoadSettings<DataSettings>()); }
        }

        public IDatabaseProvider DataProvider
        {
            get { return _dataProvider ?? (_dataProvider = LoadDataProvider()); }
        }

        protected IDatabaseProvider LoadDataProvider()
        {
            var providerName = DataSettings.DataProvider;
            Guard.MustFollow(providerName.HasValue(),
                () => { throw new Saturn72Exception("Data Settings is missing ProviderName"); });

            switch (providerName.ToLowerInvariant())
            {
                case "sqlserver":
                    return new SqlServerDataProvider();
                default:
                    throw new Saturn72Exception($"Not supported dataprovider name: {providerName}");
            }
        }
    }
}