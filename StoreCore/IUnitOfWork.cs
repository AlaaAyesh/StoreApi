using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreCore.Entities;
using StoreCore.RepositoriesContract;
namespace StoreCore
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
        // Create Repositories  
        IGenericRepository<TEntity, TKey> Repository<TEntity, TKey>()
            where TEntity : BaseEntity<TKey>;
    }
}
