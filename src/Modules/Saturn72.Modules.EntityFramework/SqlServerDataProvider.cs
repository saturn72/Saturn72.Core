using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using Saturn72.Core;
using Saturn72.Modules.EntityFramework.Initializers;

namespace Saturn72.Modules.EntityFramework
{
    public class SqlServerDataProvider : IDatabaseProvider
    {
        public virtual void SetDatabaseInitializer()
        {
            var initializer = new CreateDatabaseAndTableIfNotExists<Saturn72ObjectContext>(GetMandatoryTables(),
                GetSqlScripts());

            Database.SetInitializer(initializer);
        }

        private string[] GetSqlScripts()
        {
            return null;
        }

        private string[] GetMandatoryTables()
        {
            return new[]
            {
                "Setting",
                "LocaleStringResource"
            };
        }

        public virtual Type GetUnproxiedEntityType(BaseEntity entity)
        {
            var userType = ObjectContext.GetObjectType(entity.GetType());
            return userType;
        }
    }
}