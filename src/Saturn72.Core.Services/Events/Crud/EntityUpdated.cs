namespace Saturn72.Core.Services.Events
{
    public class EntityUpdated<T> : BaseCrudEvent<T> where T : BaseEntity
    {
        public EntityUpdated(T entity) : base(entity)
        {
        }
    }
}