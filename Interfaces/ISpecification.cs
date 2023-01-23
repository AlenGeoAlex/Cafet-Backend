using System.Linq.Expressions;

namespace Cafet_Backend.Interfaces;

public interface ISpecification<TEntity>
{
    // Filter Conditions
    List<Expression<Func<TEntity, bool>>> FilterCondition { get; }
    
    // Order By Ascending
    Expression<Func<TEntity, object>>? OrderBy { get; }
    
    // Order By Descending
    Expression<Func<TEntity, object>>? OrderByDescending { get; }
    
    // Include collection to load related data
    List<Expression<Func<TEntity, object>>>? Includes { get; }
    
    // GroupBy expression
    Expression<Func<TEntity, object>>? GroupBy { get; }
    
    int Limit { get; }
}