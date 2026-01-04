using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreCore.Entities;
using StoreCore.Specificatons;

namespace StoreRepository
{
    public class SpecificationEvaluator<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        public static IQueryable<TEntity> GetQuery<TEntity, TKey>(IQueryable<TEntity> inputQuery, ISpecifications<TEntity, TKey> specifications) where TEntity : StoreCore.Entities.BaseEntity<TKey>
        {
            var query = inputQuery;
            if (specifications.Criteria != null)
            {
                query = query.Where(specifications.Criteria);
            }
            if (specifications.OrderBy != null)
            {
                query = query.OrderBy(specifications.OrderBy);
            }
            if (specifications.OrderByDescending != null)
            {
                query = query.OrderByDescending(specifications.OrderByDescending);
            }
            if (specifications.IsPagingEnabled)
            {
                query = query.Skip(specifications.Skip).Take(specifications.Take);
            }

            // there are two ways to include related entities in EF Core: 
            // 1. Using Include method
            //if (specifications.Includes != null && specifications.Includes.Any())
            //{
            //    foreach (var include in specifications.Includes)
            //    {
            //        query = query.Include(include);
            //    }
            //}

            // 2. Using Aggregate method to include all related entities in a single line
            //aggregate includes
            query = specifications.Includes.Aggregate(query, (current, include) => current.Include(include));


            return query;
        }
    }
}
