﻿namespace Saturn72.Core.Services.Events.Crud
{
    public class EntityUpdated<T> : BaseCrudEvent<T> where T : BaseEntity
    {
        public EntityUpdated(T entity) : base(entity)
        {
        }
    }
}