using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Transactions;

namespace Saturn72.Modules.EntityFramework.Initializers
{
    public class CreateTablesIfNotExist<TContext> : IDatabaseInitializer<TContext> where TContext : DbContext
    {
        private readonly string[] _mandatoryTables;
        private readonly string[] _sqlCommands;

        public CreateTablesIfNotExist(string[] mandatoryTables, string[] sqlCommands)
        {
            _mandatoryTables = mandatoryTables;
            _sqlCommands = sqlCommands;
        }

        public virtual void InitializeDatabase(TContext context)
        {
            bool dbExists;
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                dbExists = context.Database.Exists();
            }

            if (dbExists)
            {
                bool shouldCreateTables;
                if (_mandatoryTables != null && _mandatoryTables.Length > 0)
                {
                    //we have some table names to validate
                    var existingTableNames =
                        new List<string>(
                            context.Database.SqlQuery<string>(
                                "SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE'"));

                    shouldCreateTables =
                        !existingTableNames.Intersect(_mandatoryTables, StringComparer.InvariantCultureIgnoreCase).Any();
                }
                else
                {
                    //check whether tables are already created
                    var numberOfTables = 0;
                    foreach (
                        var t1 in
                            context.Database.SqlQuery<int>(
                                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE' "))
                        numberOfTables = t1;

                    shouldCreateTables = numberOfTables == 0;
                }

                if (shouldCreateTables)
                    CreateTables(context);
            }
            else
            {
                throw new ApplicationException("No database instance");
            }
        }

        private void CreateTables(TContext context)
        {
            //create all tables
            var dbCreationScript = ((IObjectContextAdapter) context).ObjectContext.CreateDatabaseScript();
            context.Database.ExecuteSqlCommand(dbCreationScript);

            //Seed(context);
            context.SaveChanges();

            if (_sqlCommands != null && _sqlCommands.Length > 0)
            {
                foreach (var command in _sqlCommands)
                    context.Database.ExecuteSqlCommand(command);
            }
        }
    }
}