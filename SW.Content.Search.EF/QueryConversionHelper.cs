using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SW.Content.Search.EF
{
    public static class QueryConversionHelper
    {
        //static string GetTypeFieldName(ContentType type)
        //{
        //    switch (type)
        //    {
        //        case ContentType.Boolean: return nameof(DbDocToken.ValueAsBoolean);
        //        case ContentType.DateTime: return nameof(DbDocToken.ValueAsDateTime);
        //        case ContentType.Number: return nameof(DbDocToken.ValueAsNumber);
        //        case ContentType.Text: return nameof(DbDocToken.ValueAsString);
        //        case ContentType.Null: return nameof(DbDocToken.ValueAsAny);
        //        default: throw new NotSupportedException("content type not supported");
        //    }
        //}

        //static Expression Process(SearchQuery.Line line, ParameterExpression p)
        //{
            

        //    Expression body = Expression.Constant(true);

        //    var path = line.Field.ToString();
        //    var lhs = Expression.PropertyOrField(
        //        Expression.PropertyOrField(p, nameof(DbDocToken.Path)),
        //        nameof(DbDocSourcePath.PathString));
        //    var rhs = Expression.Constant(path, typeof(string));
        //    var pathEq = Expression.Equal(lhs, rhs);


        //    Expression valueComp;
        //    if (line.Comparison == SearchQuery.Op.AnyOf)
        //    {

        //        valueComp = Expression.Constant(false);
        //        var list = line.Value as ContentList;
        //        foreach (var item in list)
        //        {
        //            valueComp = Expression.OrElse(
        //                valueComp,
        //                Process(new SearchQuery.Line(line.Field, SearchQuery.Op.Equals, item), p));
        //        }
        //    }
        //    else
        //    {
        //        var valueType = line.Value.ContentType();
        //      //  var compareWith = GetTypeFieldName(valueType);

        //        Expression constant = Expression.Constant(null);
        //        if (line.Value is ContentText text) constant = Expression.Constant(text.Value.ToLowerInvariant());
        //        else if (line.Value is ContentNumber num) constant = Expression.Constant(num.Value, typeof(decimal?));
        //        else if (line.Value is ContentBoolean b) constant = Expression.Constant(b.Value, typeof(bool?));
        //        else if (line.Value is ContentDateTime dt) constant = Expression.Constant(dt.Value, typeof(DateTime?));

        //        var left = Expression.PropertyOrField(p);


        //        switch (line.Comparison)
        //        {
        //            case SearchQuery.Op.NotEquals:
        //                valueComp = Expression.NotEqual(left, constant);
        //                break;

        //            case SearchQuery.Op.Equals:
        //                valueComp = Expression.Equal(left, constant);
        //                break;

        //            case SearchQuery.Op.GreaterThan:
        //                valueComp = Expression.GreaterThan(left, constant);
        //                break;

        //            case SearchQuery.Op.GreaterThanOrEquals:
        //                valueComp = Expression.GreaterThanOrEqual(left, constant);
        //                break;

        //            case SearchQuery.Op.LessThan:
        //                valueComp = Expression.LessThan(left, constant);
        //                break;

        //            case SearchQuery.Op.LessThanOrEquals:
        //                valueComp = Expression.LessThanOrEqual(left, constant);
        //                break;


        //            default:
        //                throw new InvalidOperationException($"Op '{line.Comparison}' is not supported");
        //        }
        //    }

        //    return Expression.AndAlso(pathEq, valueComp);
        //}

        //public static Expression BuildCriteriaExpr(SearchQuery.Line line)
        //{
            

        //    var p = Expression.Parameter(typeof(DbDocToken));
            
        //    return Expression.Lambda(Process(line, p), p);
        //}

        //public static IQueryable<DocSortValue> BuildSortBy(SearchQuery query, IQueryable<DocSortValue> q)
        //{
        //    var columnName = nameof(DbDocToken.ValueAsAny);
        //    ParameterExpression parameter = Expression.Parameter(q.ElementType);
        //    MemberExpression property = Expression.Property(
        //        Expression.PropertyOrField(parameter, nameof(DocSortValue.OrderBy)),
        //        columnName);
        //    LambdaExpression lambda = Expression.Lambda(property, parameter);
        //    string methodName = query.SortByDescending
        //        ? nameof(Enumerable.OrderByDescending)
        //        : nameof(Enumerable.OrderBy);
        //    Expression methodCallExpression = Expression.Call(typeof(Queryable), methodName,
        //                          new Type[] { q.ElementType, property.Type },
        //                          q.Expression, Expression.Quote(lambda));
        //    return q.Provider.CreateQuery<DocSortValue>(methodCallExpression);
        //}

    }
}
