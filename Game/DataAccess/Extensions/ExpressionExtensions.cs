using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Navislamia.Game.DataAccess.Extensions;

public static class ExpressionExtensions
{
    // Given an expression for a method that takes in a single parameter (and
    // returns a bool), this method converts the parameter type of the parameter
    // from TSource to TTarget.
    public static Expression<Func<TTarget, bool>> Convert<TSource, TTarget>(
        this Expression<Func<TSource, bool>> root)
    {
        var visitor = new ParameterTypeVisitor<TSource, TTarget>();
        return (Expression<Func<TTarget, bool>>)visitor.Visit(root);
    }
}

internal class ParameterTypeVisitor<TSource, TTarget> : ExpressionVisitor
{
    private ReadOnlyCollection<ParameterExpression> _parameters;

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return _parameters?.FirstOrDefault(p => p.Name == node.Name)
               ?? (node.Type == typeof(TSource) ? Expression.Parameter(typeof(TTarget), node.Name) : node);
    }

    protected override Expression VisitLambda<T>(Expression<T> node)
    {
        _parameters = VisitAndConvert(node.Parameters, "VisitLambda");
        return Expression.Lambda(Visit(node.Body), _parameters);
    }
}
