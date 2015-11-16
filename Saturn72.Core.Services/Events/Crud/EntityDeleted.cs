namespace Saturn72.Core.Services.Events
{
    public class EntityDeleted<T> : BaseCrudEvent<T> where T : BaseEntity
    {
        public EntityDeleted(T entity) : base(entity)
        {
        }
    }
}