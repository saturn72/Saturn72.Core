This module adds sql server entityframework to you project.
you need to perform tasks to enable the module:
1. Add the data provider and connection string to your ~/App_Data/Settings.txt file in the following format:
#Place this file in Main server App_Data folder
DataProvider: <your_data_provider_name_goes_here>
DataConnectionString: Data Source=<your_connection_String_goes_here>

2. Map all your entities using  Saturn72.Modules.EntityFramework.Mapping.EfEntityTypeConfiguration<T>
see  Saturn72.Modules.EntityFramework.Mapping folder for examples