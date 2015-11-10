using System;
using System.Collections.Generic;
using System.Linq;

namespace Automation.Core.Data
{
    public static class RepositoryExtensions
    {
        public static IEnumerable<TEntity> GetBy<TEntity>(this IRepository<TEntity> repository, Func<TEntity, bool> func)
            where TEntity : BaseEntity
        {
            return repository.Table.Where(func);
        }
    }
}
