using SolveStation.Common.Interfaces;

namespace SolveStation.Common.Specifications;

/// <summary>
/// Base specification implementation
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public abstract class BaseSpecification<T> : ISpecification<T>
{
    public abstract bool IsSatisfiedBy(T entity);
    public abstract IQueryable<T> Apply(IQueryable<T> query);

    public static ISpecification<T> And(ISpecification<T> left, ISpecification<T> right)
    {
        return new AndSpecification<T>(left, right);
    }

    public static ISpecification<T> Or(ISpecification<T> left, ISpecification<T> right)
    {
        return new OrSpecification<T>(left, right);
    }

    public static ISpecification<T> Not(ISpecification<T> specification)
    {
        return new NotSpecification<T>(specification);
    }
}

/// <summary>
/// AND specification combinator
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class AndSpecification<T> : BaseSpecification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override bool IsSatisfiedBy(T entity)
    {
        return _left.IsSatisfiedBy(entity) && _right.IsSatisfiedBy(entity);
    }

    public override IQueryable<T> Apply(IQueryable<T> query)
    {
        return _right.Apply(_left.Apply(query));
    }
}

/// <summary>
/// OR specification combinator
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class OrSpecification<T> : BaseSpecification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public OrSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override bool IsSatisfiedBy(T entity)
    {
        return _left.IsSatisfiedBy(entity) || _right.IsSatisfiedBy(entity);
    }

    public override IQueryable<T> Apply(IQueryable<T> query)
    {
        // For OR operations in LINQ, we need to use Union
        return _left.Apply(query).Union(_right.Apply(query));
    }
}

/// <summary>
/// NOT specification combinator
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class NotSpecification<T> : BaseSpecification<T>
{
    private readonly ISpecification<T> _specification;

    public NotSpecification(ISpecification<T> specification)
    {
        _specification = specification;
    }

    public override bool IsSatisfiedBy(T entity)
    {
        return !_specification.IsSatisfiedBy(entity);
    }

    public override IQueryable<T> Apply(IQueryable<T> query)
    {
        // For NOT operations, we need to implement inverse logic
        // This is a simplified implementation - in practice, you might need more complex logic
        return query.Where(x => !_specification.IsSatisfiedBy(x));
    }
}
