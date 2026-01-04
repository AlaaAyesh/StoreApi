using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreCore;
using StoreCore.Entities;
using StoreCore.RepositoriesContract;
using StoreRepository.Data.Contexts;
using StoreRepository.Repositoies;

namespace StoreRepository
{
    public class UnitOfWorkcs : IUnitOfWork
    {
        private readonly StoreDbContext _context;
        private Hashtable _repositories;

        public UnitOfWorkcs(StoreDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _repositories = new Hashtable();
        }
        public async Task CompleteAsync()
        {
             await _context.SaveChangesAsync();
        }

        public IGenericRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryInstance = new GenericRepository<TEntity, TKey>(_context);
                _repositories.Add(type, repositoryInstance);
            }

            return _repositories[type] as IGenericRepository<TEntity, TKey>
                   ?? throw new InvalidOperationException($"Repository for type {type} not found.");
        }

    }
}
