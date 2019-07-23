using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SW.Content;
using SW.Content.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SW.Content.Search.EF
{
    public class SearchQueryHandler<TDoc> : ISearchQueryHandler
    {

        

        class DocSortValue
        {
            public DbDoc Doc { get; set; }

            public DbDocToken OrderBy { get; set; }
        }

        static readonly MethodInfo _anyMethod;

        static SearchQueryHandler()
        {
            //Create Generic Any Method
            Func<MethodInfo, bool> methodLambda = m => m.Name == nameof(Enumerable.Any) && m.GetParameters().Length == 2;
            _anyMethod = typeof(Enumerable).GetMethods().Where(methodLambda).Single().MakeGenericMethod(typeof(DbDocToken));
        }
        
        readonly DbContext _dbc;

        public SearchQueryHandler(DbContext dbc)
        {
            _dbc = dbc;
        }
        
        static string GetTypeFieldName(ContentType type)
        {
            switch (type)
            {
                case ContentType.Boolean: return nameof(DbDocToken.ValueAsBoolean);
                case ContentType.DateTime: return nameof(DbDocToken.ValueAsDateTime);
                case ContentType.Number: return nameof(DbDocToken.ValueAsNumber);
                case ContentType.Text: return nameof(DbDocToken.ValueAsString);
                case ContentType.Null: return nameof(DbDocToken.ValueAsAny);
                default: throw new NotSupportedException("content type not supported");
            }
        }

        static IQueryable<DocSortValue> BuildSortBy(SearchQuery query, IQueryable<DocSortValue> q)
        {
            var columnName = nameof(DbDocToken.ValueAsAny);
            ParameterExpression parameter = Expression.Parameter(q.ElementType);
            MemberExpression property = Expression.Property(
                Expression.PropertyOrField(parameter, nameof(DocSortValue.OrderBy)), 
                columnName);
            LambdaExpression lambda = Expression.Lambda(property, parameter);
            string methodName = query.SortByDescending 
                ? nameof(Enumerable.OrderByDescending) 
                : nameof(Enumerable.OrderBy);
            Expression methodCallExpression = Expression.Call(typeof(Queryable), methodName,
                                  new Type[] { q.ElementType, property.Type },
                                  q.Expression, Expression.Quote(lambda));
            return q.Provider.CreateQuery<DocSortValue>(methodCallExpression);
        }
        
        Expression BuildCriteriaExpr(SearchQuery.Line line)
        {
            var valueType = line.Value.ContentType();
            
            var p = Expression.Parameter(typeof(DbDocToken));
            Expression body = Expression.Constant(true);
            
            var path = line.Field.ToString();
            var lhs = Expression.PropertyOrField(
                Expression.PropertyOrField(p, nameof(DbDocToken.Path)),
                nameof(DbDocSourcePath.PathString));
            var rhs = Expression.Constant(path, typeof(string));
            var pathEq = Expression.Equal(lhs, rhs);
            
            
            Expression valueComp;
            if (line.Comparison == SearchQuery.Op.AnyOf)
            {
                
                valueComp = Expression.Constant(false);
                var list = line.Value as ContentList;
                foreach (var item in list)
                {
                    valueComp = Expression.OrElse(
                        valueComp,
                        BuildCriteriaExpr(new SearchQuery.Line(line.Field, SearchQuery.Op.Equals, item)));
                }
            }
            else
            {
                var compareWith = GetTypeFieldName(valueType);

                Expression constant = Expression.Constant(null);
                if (line.Value is ContentText text) constant = Expression.Constant(text.Value.ToLowerInvariant());
                else if (line.Value is ContentNumber num) constant = Expression.Constant(num.Value, typeof(decimal?));
                else if (line.Value is ContentBoolean b) constant = Expression.Constant(b.Value, typeof(bool?));
                else if (line.Value is ContentDateTime dt) constant = Expression.Constant(dt.Value, typeof(DateTime?));

                var left = Expression.PropertyOrField(p, compareWith);

                
                switch (line.Comparison)
                {
                    case SearchQuery.Op.Equals:
                        valueComp = Expression.Equal(left, constant);
                        break;

                    case SearchQuery.Op.GreaterThan:
                        valueComp = Expression.GreaterThan(left, constant);
                        break;

                    case SearchQuery.Op.GreaterThanOrEquals:
                        valueComp = Expression.GreaterThanOrEqual(left, constant);
                        break;

                    case SearchQuery.Op.LessThan:
                        valueComp = Expression.LessThan(left, constant);
                        break;

                    case SearchQuery.Op.LessThanOrEquals:
                        valueComp = Expression.LessThanOrEqual(left, constant);
                        break;


                    default:
                        throw new InvalidOperationException($"Op '{line.Comparison}' is not supported");
                }
            }
            
            return Expression.Lambda(Expression.AndAlso(pathEq, valueComp), p);
        }
        

        public async Task<SearchQueryResult<T>> Handle<T>(SearchQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            var docTypeName = query.DocumentType.Name;
            var paths = await _dbc.Set<DbDocSourcePath>()
                .Where(row => row.DocumentType == docTypeName)
                .ToDictionaryAsync(o => o.PathString);

            // create a sortable model

            var sortPath = query.SortByField.ToString();
            IQueryable<DocSortValue> q = _dbc.Set<DbDoc>()
                .Select(d => new DocSortValue
                {
                    Doc = d,
                    OrderBy = d.Tokens
                        .Where(t => t.Path.PathString == sortPath)
                        .FirstOrDefault()
                });
            
            // query filters
            
            var p = Expression.Parameter(typeof(DocSortValue));
            Expression body = Expression.Constant(true);
            foreach (var line in query.QueryLines)
            {
                var tokenList = Expression.PropertyOrField(
                    Expression.PropertyOrField(p, nameof(DocSortValue.Doc)),
                    nameof(DbDoc.Tokens));

                var callAny = Expression.Call(_anyMethod, tokenList, BuildCriteriaExpr(line));
                body = Expression.AndAlso(body, callAny);
            }

            var where = Expression.Lambda<Func<DocSortValue, bool>>(body, p);
            q = q.Where(where).Where(d => d.Doc.SourceType == docTypeName);
            
            // apply sorting and pagination

            q = BuildSortBy(query, q);
            var count = await q.CountAsync();
            var matches = await q.Skip(query.Offset).Take(query.Limit).ToArrayAsync();
            
            return new SearchQueryResult<T>(matches
                .Select(m => JsonUtil.Deserialize<T>(m.Doc.BodyData)).ToArray(), 
                count);
        }
    }
}
