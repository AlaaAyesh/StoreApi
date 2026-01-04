using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreCore.Entities;
using StoreCore.Entities.Order;
using StoreCore.Specificatons;

namespace StoreCore.RepositoriesContract
{
    public interface IGenericRepository<TEntity, TKey> where TEntity :BaseEntity<TKey>
    {
        Task <IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(TKey id);

        Task<IEnumerable<TEntity>> GetAllWithSpecificationAsync(ISpecifications < TEntity, TKey> spec);
        Task<TEntity> GetByIdWithSpecificationAsync(ISpecifications<TEntity, TKey> spec);
        Task<int> GetCountAsync(ISpecifications<TEntity, TKey> spec);

        Task AddAsync(TEntity entity);

        //uint Update(TEntity entity);
        void Delete(TEntity entity);

    }
}
