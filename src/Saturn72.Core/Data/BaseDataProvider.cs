using System;

namespace Saturn72.Core.Data
{
    public abstract class BaseDataProvider
    {
        public abstract void SetDatabaseInitializer();

        public virtual void InitDatabase()
        {
            SetDatabaseInitializer();
        }

        public abstract Type GetUnproxiedEntityType(BaseEntity entity);
    }
}