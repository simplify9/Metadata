using Microsoft.EntityFrameworkCore;
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
            Func<MethodInfo, bool> methodLambda = m => m.Name == "Any" && m.GetParameters().Length == 2;
            _anyMethod = typeof(Enumerable).GetMethods().Where(methodLambda).Single().MakeGenericMethod(typeof(DbDocToken));
        }
        
        readonly DbContext _dbc;

        public SearchQueryHandler(DbContext dbc)
        {
            _dbc = dbc;
        }

        //static ContentType FindContentType(Type docType, ContentPath fieldPath)
        //{
        //    var sortFieldType = docType;
        //    foreach (var n in fieldPath)
        //    {
        //        sortFieldType = sortFieldType.GetProperty(n).PropertyType;
        //    }

        //    ContentType t = ContentType.Number;
        //    if (sortFieldType == typeof(string)) t = ContentType.Text;
        //    else if (sortFieldType == typeof(bool) || sortFieldType == typeof(bool?)) t = ContentType.Boolean;
        //    else if (sortFieldType == typeof(DateTime) || sortFieldType == typeof(DateTime?)) t = ContentType.DateTime;

        //    return t;
        //}
        
        static string GetTypeFieldName(ContentType type)
        {
            switch (type)
            {
                case ContentType.Boolean: return nameof(DbDocToken.ValueAsBoolean);
                case ContentType.DateTime: return nameof(DbDocToken.ValueAsDateTime);
                case ContentType.Number: return nameof(DbDocToken.ValueAsNumber);
                case ContentType.Text: return nameof(DbDocToken.ValueAsString);
                default: throw new NotSupportedException("content type not supported");
            }
        }

        static IQueryable<DocSortValue> BuildSortBy(SearchQuery query, IQueryable<DocSortValue> q)
        {
            //var leafTypes = query.DocumentType.GetLeafContentTypes(query.SortByField);

            var schema = query.DocumentType.Schema;
            
            var columnName = nameof(DbDocToken.ValueAsAny);
            ParameterExpression parameter = Expression.Parameter(q.ElementType);
            MemberExpression property = Expression.Property(
                Expression.PropertyOrField(parameter, nameof(DocSortValue.OrderBy)), 
                columnName);
            LambdaExpression lambda = Expression.Lambda(property, parameter);
            string methodName = query.SortByDescending ? "OrderByDescending": "OrderBy";
            Expression methodCallExpression = Expression.Call(typeof(Queryable), methodName,
                                  new Type[] { q.ElementType, property.Type },
                                  q.Expression, Expression.Quote(lambda));
            return q.Provider.CreateQuery<DocSortValue>(methodCallExpression);
        }
 
        //static ContentType[] ResolveOrderByContentType(IMust docSchema, ContentPath path)
        //{
        //    if (docSchema.TryGetSchema(path, out IMust fieldSchema))
        //    {
        //        //fieldSchema.
        //    }
            
        //}
        
        Expression BuildCriteriaExpr(SearchQuery query, SearchQuery.Line line)
        {
            var valueType = ((IContentNode)line.Value).ContentType();
            
            var p = Expression.Parameter(typeof(DbDocToken));
            Expression body = Expression.Constant(true);
            


            var path = line.Field.ToString();
            var lhs = Expression.PropertyOrField(
                Expression.PropertyOrField(p, nameof(DbDocToken.Path)),
                nameof(DbDocSourcePath.PathString));
            var rhs = Expression.Constant(path, typeof(string));
            var pathEq = Expression.Equal(lhs, rhs);
            
            var compareWith = GetTypeFieldName(valueType);

            Expression constant = Expression.Constant(null);
            if (line.Value is ContentText text) constant = Expression.Constant(text.Value);
            else if (line.Value is ContentNumber num) constant = Expression.Constant(num.Value, typeof(decimal?));
            else if (line.Value is ContentBoolean b) constant = Expression.Constant(b.Value, typeof(bool?));
            else if (line.Value is ContentDateTime dt) constant = Expression.Constant(dt.Value, typeof(DateTime?));
            
            var valueEq = Expression.Equal(Expression.PropertyOrField(p, compareWith), constant);
            return Expression.Lambda(Expression.AndAlso(pathEq, valueEq), p);
        }
        

        public async Task<SearchQueryResult> Handle(SearchQuery query)
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

                var callAny = Expression.Call(_anyMethod, tokenList, BuildCriteriaExpr(query, line));
                body = Expression.AndAlso(body, callAny);
            }

            var where = Expression.Lambda<Func<DocSortValue, bool>>(body, p);
            q = q.Where(where);
            
            // apply sorting and pagination

            q = BuildSortBy(query, q);
            var count = await q.CountAsync();
            var matches = await q.Skip(query.Offset).Take(query.Limit).ToArrayAsync();

            // format result

            return new SearchQueryResult(matches
                .Select(m => JToken.FromObject(m.Doc.BodyData)).ToArray(), 
                count);
        }
    }
}
