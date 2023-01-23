using System.Linq.Expressions;
using Cafet_Backend.Interfaces;

namespace Cafet_Backend.Specification;

public class Specification<TEntity> : ISpecification<TEntity>
{
    private readonly List<Expression<Func<TEntity, object>>> _includeCollection = new List<Expression<Func<TEntity, object>>>();

    public Specification()
    {
        this.FilterCondition = new List<Expression<Func<TEntity, bool>>>();
    }

    public Specification(Expression<Func<TEntity, bool>> filterCondition)
    {
        this.FilterCondition = new List<Expression<Func<TEntity, bool>>>();
    }

    public List<Expression<Func<TEntity, bool>>> FilterCondition { get; private set; }
    public Expression<Func<TEntity, object>> OrderBy { get; private set; }
    public Expression<Func<TEntity, object>> OrderByDescending { get; private set; }
    public List<Expression<Func<TEntity, object>>> Includes
    {
        get
        {
            return _includeCollection;
        }
    }

    public Expression<Func<TEntity, object>> GroupBy { get; private set; }
    public int Limit { get; set; }

    public void AddInclude(Expression<Func<TEntity, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    public void ApplyOrderBy(Expression<Func<TEntity, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    public void ApplyOrderByDescending(Expression<Func<TEntity, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }

    public void AddFilterCondition(Expression<Func<TEntity, bool>> filterExpression)
    {
        FilterCondition.Add(filterExpression);
    }

    public void ApplyGroupBy(Expression<Func<TEntity, object>> groupByExpression)
    {
        GroupBy = groupByExpression;
    }
}