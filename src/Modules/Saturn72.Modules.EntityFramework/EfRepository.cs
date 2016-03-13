using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using Saturn72.Core;
using Saturn72.Core.Data;
using Saturn72.Extensions;

namespace Saturn72.Modules.EntityFramework
{
    public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        #region Ctor

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public EfRepository(IDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Fields

        private readonly IDbContext _context;
        private IDbSet<TEntity> _entities;

        #endregion

        #region Methods

        /// <summary>
        ///     Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public virtual TEntity GetById(object id)
        {
            //see some suggested performance optimization (not tested)
            //http://stackoverflow.com/questions/11686225/dbset-find-method-ridiculously-slow-compared-to-singleordefault-on-id/11688189#comment34876113_11688189
            return Entities.Find(id);
        }

        /// <summary>
        ///     Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Insert(TEntity entity)
        {
            Guard.NotNull(entity, nameof(entity));

            var insertEntityAction = new Action(() =>
            {
                Entities.Add(entity);
                Entities.Add(HandleCreateableAndUpdateableEntity(entity));
                _context.SaveChanges();
            });
            TryCatchDatabaseAction(insertEntityAction);
        }

        protected virtual TEntity HandleCreateableAndUpdateableEntity(TEntity entity)
        {
            var createableEntity = entity as ICreateableEntity;
            if (createableEntity.NotNull() && createableEntity.CreatedOnUtc == DateTime.MinValue)
                createableEntity.CreatedOnUtc = DateTime.UtcNow;

            var updateableEntity = entity as IUpdateableEntity;
            if (updateableEntity.NotNull())
                updateableEntity.UpdatedOnUtc = DateTime.UtcNow;

            return entity;
        }

        /// <summary>
        ///     Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Insert(IEnumerable<TEntity> entities)
        {
            Guard.NotEmpty(entities);

            var insertMultiEntities = new Action(() =>
            {
                foreach (var entity in entities)
                {
                    Entities.Add(entity);
                    Entities.Add(HandleCreateableAndUpdateableEntity(entity));
                }

                _context.SaveChanges();
            });
            TryCatchDatabaseAction(insertMultiEntities);
        }

        /// <summary>
        ///     Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Update(TEntity entity)
        {
            Guard.NotNull(entity);
            var deleteAction = new Action(() =>
            {
                Entities.Add(HandleCreateableAndUpdateableEntity(entity));
                _context.SaveChanges();
            });

            TryCatchDatabaseAction(deleteAction);
        }

        /// <summary>
        ///     Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Delete(TEntity entity)
        {
            Guard.NotNull(entity);
            var deleteEntityAction = new Action(() =>
            {
                Entities.Remove(entity);
                _context.SaveChanges();
            });

            TryCatchDatabaseAction(deleteEntityAction);
        }

        /// <summary>
        ///     Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Delete(IEnumerable<TEntity> entities)
        {
            Guard.NotEmpty(entities);
            var deleteMultiAction = new Action(() =>
            {
                foreach (var entity in entities)
                    Entities.Remove(entity);
                _context.SaveChanges();
            });

            TryCatchDatabaseAction(deleteMultiAction);
        }

        private static void TryCatchDatabaseAction(Action databaseAction)
        {
            try
            {
                databaseAction();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine +
                               string.Format("Property: {0} Error: {1}", validationError.PropertyName,
                                   validationError.ErrorMessage);

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a table
        /// </summary>
        public virtual IQueryable<TEntity> Table
        {
            get { return Entities; }
        }

        /// <summary>
        ///     Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only
        ///     operations
        /// </summary>
        public virtual IQueryable<TEntity> TableNoTracking => Entities.AsNoTracking();

        /// <summary>
        ///     Entities
        /// </summary>
        protected virtual IDbSet<TEntity> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<TEntity>();
                return _entities;
            }
        }

        #endregion
    }
}