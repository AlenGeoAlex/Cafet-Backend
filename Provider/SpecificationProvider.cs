using System.Linq.Expressions;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Cafet_Backend.Provider;

public class SpecificationProvider<TEntity, TIdentifier> where TEntity : KeyedEntity<TIdentifier>
{
    public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> query, ISpecification<TEntity>? specifications)
    {
        // Do not apply anything if specifications is null
        if (specifications == null)
        {
            return query;
        }
        
        // Modify the IQueryable
        // Apply filter conditions
        
        //query = specifications.FilterCondition.Aggregate(query, (curr, where) => curr.Where(where));

        if (specifications.FilterCondition.Count > 0)
        {
            query = specifications.FilterCondition.Aggregate(query,
                (currentState, nextQuery) => currentState.Where(nextQuery));
        }
        
        // Includes
        if (specifications.Includes != null)
        {
            query = specifications.Includes
                .Aggregate(query, (current, include) => current.Include(include));
            
        }


        if (specifications.OrderBy != null)
        {
            // Apply ordering
            query = query.OrderBy(specifications.OrderBy);
        }

        if (specifications.OrderByDescending != null)
        {
            query = query.OrderByDescending(specifications.OrderByDescending);
        }


        // Apply GroupBy
        if (specifications.GroupBy != null)
        {
            query = query.GroupBy(specifications.GroupBy).SelectMany(x => x);
        }

        
        if (specifications.Limit > 0)
        {
            query = query.Take(specifications.Limit);
        }
        
        return query;
    }
}