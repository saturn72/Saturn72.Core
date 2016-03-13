namespace Saturn72.Core.Services.Events.Crud
{
    /// <summary>
    /// A container for entities that have been inserted.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityInserted<T> :BaseCrudEvent<T> where T : BaseEntity
    {
        public EntityInserted(T entity) : base(entity)
        {
        }

    }
}