using System.Data.Entity.ModelConfiguration;

namespace Saturn72.Modules.EntityFramework.Mapping
{
    public abstract class EfEntityTypeConfiguration<TEntity> : EntityTypeConfiguration<TEntity>
        where TEntity : class
    {
        protected EfEntityTypeConfiguration()
        {
            PreInitialize();
            Initialize();
            PostInitialize();
        }

        protected abstract void Initialize();

        protected virtual void PreInitialize()
        {
        }

        protected virtual void PostInitialize()
        {
        }
    }
}