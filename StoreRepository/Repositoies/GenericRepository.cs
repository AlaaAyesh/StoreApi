using Microsoft.EntityFrameworkCore;
using StoreCore.Entities;
using StoreCore.RepositoriesContract;
using StoreCore.Specificatons;
using StoreRepository.Data.Contexts;

namespace StoreRepository.Repositoies
{
    public class GenericRepository<TEntity, Tkey> : IGenericRepository<TEntity, Tkey> where TEntity : BaseEntity<Tkey>
    {
        private readonly StoreDbContext _context;
        public GenericRepository(StoreDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task AddAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            _context.Set<TEntity>().Add(entity);
            return _context.SaveChangesAsync();
        }

        public void Delete(TEntity entity)
        {
           _context.Remove(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            if (typeof(TEntity) == typeof(Product))
            {
                return await _context.Set<TEntity>()
                    .Include(p => ((Product)(object)p).Brand!)
                    .Include(p => ((Product)(object)p).Type!)
                    .ToListAsync();
            }
            return await _context.Set<TEntity>().ToListAsync();
        }

        public  async Task<IEnumerable<TEntity>> GetAllWithSpecificationAsync(ISpecifications<TEntity, Tkey> spec)
        {
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }
            return await ApplySpecifications(spec).ToListAsync();
            

        }

        public async Task<TEntity?> GetByIdAsync(Tkey id)
        {
            if (typeof(TEntity) == typeof(Product))
            {

                // lambda experssion = p => p.Id!.Equals(id)
                return await _context.Set<TEntity>()
                    .Include(p => ((Product)(object)p).Brand!)
                    .Include(p => ((Product)(object)p).Type!)
                    .FirstOrDefaultAsync(e => e.Id!.Equals(id));
            }
            return await _context.Set<TEntity>().FirstOrDefaultAsync(e => e.Id!.Equals(id));
        }

        public async Task<TEntity> GetByIdWithSpecificationAsync(ISpecifications<TEntity, Tkey> spec)
        {
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec) );
            }
            return await ApplySpecifications(spec).FirstOrDefaultAsync();
           
        }

        public Task<int> GetCountAsync(ISpecifications<TEntity, Tkey> spec)
        {
           return  ApplySpecifications(spec).CountAsync();
        }

        public uint Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            _context.Set<TEntity>().Update(entity);
            return (uint)_context.SaveChanges();

        }

        private IQueryable<TEntity> ApplySpecifications(ISpecifications<TEntity, Tkey> spec)
        {
           return SpecificationEvaluator<TEntity, Tkey>.GetQuery(_context.Set<TEntity>(), spec);
        }
    }
}
