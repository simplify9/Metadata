using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SW.Content.Search.SearchQuery;

namespace SW.Content.Search.EF
{
    public static class SqlResolver
    {
        class Context
        {
            public Func<string,int> ResolvePath { get;  }

            public string TokenTable { get; }

            public string DocTable { get; }

            public Context(Func<string, int> resolvePath, string tokenTable, string docTable)
            {
                ResolvePath = resolvePath;
                TokenTable = tokenTable;
                DocTable = docTable;
            }
        }
        
        static string Escape(string s) => s.Replace("'", "''");

        static string FormatValue(IContentNode value)
        {
            switch (value)
            {
                case ContentNull _:
                    return "NULL";

                case IContentPrimitive prim:
                    return $"'{Escape(prim.CreateMatchKey())}'";

                case ContentList list:
                    return $"({string.Join(",", list.Select(i => FormatValue(i)))})";

                default:
                    throw new NotSupportedException($"Content node of type '{value.GetType()}' is not supported");
            }
        }

        public static string ResolveSqlText(SearchQuery query, 
            Func<string,int> pathResolver,
            string docTable,
            string tokenTable,bool withPaging=true)
        {
            var ctx = new Context(pathResolver, tokenTable, docTable);

            var docTypeName = query.DocumentType.Name;
            var filterQuery = ResolveFilterQuery(ctx, query);
            var sortPathId = pathResolver(query.SortByField.ToString());
            //var offset = query.Offset;
           // var limit = query.Limit;
            //var sorting = query.SortByDescending ? "DESC" : string.Empty;
            
            return $@"SELECT [B].* FROM (SELECT DISTINCT filtered.DocumentId, Sorted.ValueAsAny FROM 
                ({filterQuery}) as filtered 
                LEFT JOIN (SELECT DocumentId,{nameof(DbDocToken.ValueAsAny)} FROM {ctx.TokenTable}
                WHERE [PathId] = {sortPathId}) as Sorted
                ON Sorted.DocumentId = filtered.DocumentId
                ) AS [A] 
                INNER JOIN (SELECT * FROM {ctx.DocTable}
                WHERE [{nameof(DbDoc.SourceType)}] = '{docTypeName}') AS [B] ON [A].DocumentId = [B].Id";
        }

        static string ResolveFilterLine(Context ctx, SearchQuery.Line line)
        {
            var pathId = ctx.ResolvePath(line.Field.ToString());

            string op = null;
            switch (line.Comparison)
            {
                case Op.AnyOf:
                    op = " IN ";
                    break;

                case Op.Equals:
                    op = line.Value is ContentNull? " IS ": "=";
                    break;

                case Op.NotEquals:
                    op = line.Value is ContentNull? " IS NOT ": "<>";
                    break;

                case Op.GreaterThan:
                    op = ">";
                    break;

                case Op.LessThan:
                    op = "<";
                    break;

                case Op.GreaterThanOrEquals:
                    op = ">=";
                    break;

                case Op.LessThanOrEquals:
                    op = "<=";
                    break;
            }
            return $"SELECT DocumentId FROM {ctx.TokenTable} WHERE ([{nameof(DbDocToken.ValueAsAny)}]{op}{FormatValue(line.Value)} AND [PathId]={pathId})";
        }

        static string ResolveFilterQuery(Context ctx, SearchQuery query)
        {
            var docTokensSelect =$"SELECT DocumentId FROM {ctx.TokenTable} 	Group by [DocumentId]";
            return query.QueryLines.Any()
                ? $"{string.Join(" INTERSECT ", query.QueryLines.Select(li => $"{ResolveFilterLine(ctx, li)}"))}"
                : docTokensSelect;
        }
        
        
    }
}
