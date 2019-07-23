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

                var callAny = Expression.Call(_anyMethod, tokenList, QueryConversionHelper.BuildCriteriaExpr(line));
                body = Expression.AndAlso(body, callAny);
            }

            var where = Expression.Lambda<Func<DocSortValue, bool>>(body, p);
            q = q.Where(where).Where(d => d.Doc.SourceType == docTypeName);
            
            // apply sorting and pagination

            q = QueryConversionHelper.BuildSortBy(query, q);
            var count = await q.CountAsync();
            var matches = await q.Skip(query.Offset).Take(query.Limit).ToArrayAsync();
            
            return new SearchQueryResult<T>(matches
                .Select(m => JsonUtil.Deserialize<T>(m.Doc.BodyData)).ToArray(), 
                count);
        }
    }
}
