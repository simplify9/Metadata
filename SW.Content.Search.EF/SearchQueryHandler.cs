using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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
        //  private readonly SqlResolver _sqlResolver;
        readonly IEntityType docTokenModel;
        readonly IEntityType docModel;
       

        readonly string schemaName, docTokenTableName, docTableName;

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
            docTokenModel = _dbc.Model.FindRuntimeEntityType(typeof(DbDocToken));
            schemaName = docTokenModel.Relational().Schema;
            docTokenTableName = docTokenModel.Relational().TableName;
            docModel = _dbc.Model.FindRuntimeEntityType(typeof(DbDoc));
            docTableName = docModel.Relational().TableName;
        }
        
        public async Task<SearchQueryResult<T>> Handle<T>(SearchQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            var pathes = await _dbc.Set<DbDocSourcePath>().Where(p=>p.DocumentType==query.DocumentType.Name).ToListAsync();
            string withPagingstringQuery = SqlResolver.ResolveSqlText(query,(p)=>pathes.First(k=>k.PathString==p).Id,$"[{schemaName}].[{docTableName}]", $"[{schemaName}].[{docTokenTableName}]");
            string withoutSortingString = SqlResolver.ResolveSqlText(query, (p) => pathes.First(k => k.PathString == p).Id, $"[{schemaName}].[{docTableName}]", $"[{schemaName}].[{docTokenTableName}]",true);

            IQueryable<DbDoc> withoutPagingQuery = _dbc.Set<DbDoc>().FromSql(withoutSortingString);
            IQueryable<DbDoc> withPagingQuery= _dbc.Set<DbDoc>().FromSql(withPagingstringQuery);

            // compose where

            //var p = Expression.Parameter(typeof(DbDoc));
            //Expression body = Expression.Constant(true);
            //foreach (var line in query.QueryLines)
            //{
            //    var tokenList = Expression.PropertyOrField(p, nameof(DbDoc.Tokens));
            //    var callAny = Expression.Call(_anyMethod, tokenList, QueryConversionHelper.BuildCriteriaExpr(line));
            //    body = Expression.AndAlso(body, callAny);
            //}

            //var where = Expression.Lambda<Func<DbDoc, bool>>(body, p);
            //q = q.Where(where);

            //// evaluate count

            var count = await withoutPagingQuery.CountAsync();
            
            //// apply order by

            //var sortPath = query.SortByField.ToString();
            //var tokenQuery = _dbc.Set<DbDocToken>().Where(t => t.Path.PathString == sortPath);
            //var qSortable = q.GroupJoin(tokenQuery, 
            //        d => d.Id, 
            //        t => t.Document.Id, 
            //        (d,t) => new DocSortValue { Doc = d, OrderBy = t.FirstOrDefault() });
            //qSortable = QueryConversionHelper.BuildSortBy(query, qSortable);
            
            // apply pagination

            var matches = await withPagingQuery.ToArrayAsync();
            return new SearchQueryResult<T>(matches
                .Select(m => JsonUtil.Deserialize<T>(m.BodyData)).ToArray(), 
                count);

            
        }
    }
}
