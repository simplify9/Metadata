using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace SW.Content.Search.EF
{
    public class IndexDbRepo : IIndexRepo
    {
        readonly DbContext _dbc;

        // TODO support Guid keys !!

        static string GetKeyFieldName(DocumentSource source)
        {
            if (source.Key is ContentNumber n) return nameof(DbDoc.SourceIdNumber);
            if (source.Key is ContentText t) return nameof(DbDoc.SourceIdString);
            throw new NotSupportedException("Key type not supported");
        }

        static object GetKeyValue(DocumentSource source)
        {
            if (source.Key is ContentNumber n) return new long?(decimal.ToInt64(n.Value));
            if (source.Key is ContentText t) return t.Value;
            throw new NotSupportedException("Key type not supported");
        }

        static Expression CreateKeyComparison(DocumentSource source, ParameterExpression p)
        {
            Expression lhs, rhs;

            lhs = Expression.PropertyOrField(p, nameof(DbDoc.SourceType));
            rhs = Expression.Constant(source.Type.Name);
            var t1 = Expression.Equal(lhs, rhs);

            lhs = Expression.PropertyOrField(p, GetKeyFieldName(source));
            rhs = Expression.Constant(GetKeyValue(source), source.Key is ContentNumber ? typeof(long?) : typeof(string));
            var t2 = Expression.Equal(lhs, rhs);

            return Expression.AndAlso(t1, t2);
        }


        IQueryable<DbDoc> BuildDocumentSetQueryable(IEnumerable<DocumentSource> sources)
        {
            var p = Expression.Parameter(typeof(DbDoc));
            Expression body = Expression.Constant(false);
            foreach (var source in sources)
            {
                body = Expression.OrElse(body, CreateKeyComparison(source, p));
            }
            var expr = Expression.Lambda<Func<DbDoc, bool>>(body, p);
            return _dbc.Set<DbDoc>().Where(expr);
        }

        static long GetDocumentPersistenceId(IEnumerable<DbDoc> docs, DocumentSource source)
        {
            docs = docs.Where(d => d.SourceType == source.Type.Name);
            if (source.Key is ContentNumber n)
            {
                var v = decimal.ToInt64(n.Value);
                docs = docs.Where(d => d.SourceIdNumber == v);
            }
            else if (source.Key is ContentText t)
            {
                docs = docs.Where(d => d.SourceIdString == t.Value);
            }
            else
            {
                throw new NotSupportedException("Document has a key of type that is not supported");
            }

            return docs.Select(d => d.Id).FirstOrDefault();
        }

        void AddOrUpdateDocument(DocumentSource source, object sourceData)
        {
            var docs = _dbc.ChangeTracker.Entries<DbDoc>()
                .Select(e => e.Entity)
                .Where(d => d.SourceType == source.Type.Name);

            if (source.Key is ContentNumber n)
            {
                var v = decimal.ToInt64(n.Value);
                docs = docs.Where(d => d.SourceIdNumber == v);
            }
            else if (source.Key is ContentText t)
            {
                docs = docs.Where(d => d.SourceIdString == t.Value);
            }
            else
            {
                throw new NotSupportedException("Document has a key of type that is not supported");
            }

            var doc = docs.FirstOrDefault();
            if (doc == null)
            {
                doc = new DbDoc
                {
                    CreatedOn = DateTime.UtcNow,
                    SourceIdGuid = null,
                    SourceIdNumber = source.Key is ContentNumber num ? (long?)decimal.ToInt64(num.Value) : null,
                    SourceIdString = source.Key is ContentText text ? text.Value : null,
                    SourceType = source.Type.Name,
                    Tokens = new List<DbDocToken>()
                };
                _dbc.Add(doc);
            }

            doc.LastIndexOn = DateTime.UtcNow;
            doc.BodyEncoding = "application/json";
            doc.BodyData = JsonUtil.Serialize( sourceData);

        }

        void AddOrUpdatePath(DocumentToken token)
        {
            var pathString = token.SourcePath.Path.ToString();
            var path = _dbc.ChangeTracker
                .Entries<DbDocSourcePath>()
                .Select(e => e.Entity)
                .FirstOrDefault(p =>
                    p.DocumentType == token.Source.Type.Name &&
                    p.PathString == pathString);
            if (path == null)
            {
                path = new DbDocSourcePath
                {
                    CreatedOn = DateTime.UtcNow,
                    DocumentType = token.Source.Type.Name,
                    PathString = pathString
                };
                _dbc.Add(path);
            }
        }

        
        private async Task SaveTokens(DocumentToken[] newTokens, 
            DocumentToken[] oldTokens, 
            Func<DocumentToken,long> docResolver, 
            Func<DocumentToken,int> pathResolver)
        {
            var addedTokens = newTokens
                .Where(t => !oldTokens.Any(dbt => t.SourcePath.Path.Equals(dbt.SourcePath.Path) && t.Source.Equals(dbt.Source)));

            var updatedTokens = newTokens
                .Where(t => oldTokens.Any(dbt => t.SourcePath.Path.Equals(dbt.SourcePath.Path) && t.Source.Equals(dbt.Source) && !t.Raw.Equals(dbt.Raw))
                 );

            var deletedTokens = oldTokens.Where(t => !newTokens.Any(nt => nt.Source.Equals(t.Source) && t.SourcePath.Path.Equals(nt.SourcePath.Path)));
            
            var stringBuilder = new StringBuilderHelper();
            // add is 0 ,1 is update and delete is 2
            var addAndMergeUpdatesAndDeletes = addedTokens.Select(a => new Tuple<int,DocumentToken>(0,a)).Concat(
                updatedTokens.Select(u => new Tuple<int, DocumentToken>(1, u))
                .Concat(
                    deletedTokens.Select(d => new Tuple<int, DocumentToken>(2, d))
                    )
                );
            var addAndUpdateAndDeletebulks = addAndMergeUpdatesAndDeletes.ToBatch(50, t => t);
          
            foreach (var chg in addAndUpdateAndDeletebulks)
            {
                foreach (var tuple in chg)
                {
                    var token = tuple.Item2;
                    var docId = docResolver(token);
                    var pathId = pathResolver(token);
                    var offset = token.SourcePath.Offset;
                    var valueAsAny = ((IContentPrimitive)token.Raw).CreateMatchKey();
                    var lastUpdated = DateTime.UtcNow;


                    if (tuple.Item1 == 0)
                    {
                        var createdOn = DateTime.UtcNow;
                        stringBuilder.Append($@"
                        INSERT INTO [DocTokens]
                                   ([DocumentId],[PathId],[Offset],[CreatedOn],[LastUpdatedOn],[ValueAsAny])
                             VALUES
                                   ( ? , ? , ? , ? , ? , ?)",docId,pathId,offset,createdOn,lastUpdated,valueAsAny);

                    }
                    else if (tuple.Item1 == 1)
                    {
                        stringBuilder.Append($@"
                        UPDATE DocTokens
                        SET
                            LastUpdatedOn = ?,ValueAsAny = ?
                        WHERE DocumentId = ? and PathId = ? and [Offset] = ?",
                                lastUpdated,
                                valueAsAny,docId, pathId, offset);
                    }
                    else if(tuple.Item1 == 2)
                    {
                        if (pathId > 0)
                        {
                            stringBuilder.Append(@"
                            DELETE FROM DocTokens
                                WHERE [DocumentId] = ? and [PathId] = ? and [Offset] = ? ", docId , pathId, offset );
                        }
                    }
                }
             
                var comm = stringBuilder.CreateCommand(_dbc.Database.GetDbConnection());
                var u = await comm.ExecuteNonQueryAsync();
                stringBuilder.Clear();
              
            }
        }

        public async Task DeleteDocuments(DocumentSource[] sources)
        {
            var docsToDelete = BuildDocumentSetQueryable(sources).Include(d => d.Tokens);
            foreach (var doc in docsToDelete.ToList())
            {
                foreach (var docToken in doc.Tokens.ToList()) doc.Tokens.Remove(docToken);
                _dbc.Remove(doc);
            }

            await _dbc.SaveChangesAsync();
        }

        public async Task UpdateDocuments(Document[] docs)
        {
            var dbDocs = await BuildDocumentSetQueryable(docs.Select(d => d.Source))
                //.Include(d => d.Tokens)
                .ToArrayAsync();

            var oldTokens = dbDocs.SelectMany(doc =>
               TokenHelper.GetTokens(
                   new DocumentSource(
                       new DocumentType(Type.GetType(doc.SourceType)), ContentFactory.Default.CreateFrom(doc.SourceIdString)
                       ),
                   ContentFactory.Default.CreateFrom(JsonUtil.Deserialize<JToken>(doc.BodyData)).ToJson()
                   )
                ).ToArray();

            foreach (var doc in docs) AddOrUpdateDocument(doc.Source, doc.Data);


            // ensure paths
            var types = docs.Select(t => t.Source.Type.Name).Distinct();
            var paths = await _dbc.Set<DbDocSourcePath>()
                .Where(p => types.Contains(p.DocumentType))
                .ToArrayAsync();

            var newTokens = docs.SelectMany(u => TokenHelper.GetTokens(u.Source, u.Data)).ToArray();

            foreach (var t in newTokens) AddOrUpdatePath(t);

            await _dbc.SaveChangesAsync();

            var allDocs = _dbc.ChangeTracker.Entries<DbDoc>().Select(e => e.Entity).ToArray();

            var allPaths = _dbc.ChangeTracker.Entries<DbDocSourcePath>().Select(e => e.Entity).ToArray();

            await SaveTokens(newTokens, oldTokens, 
                t => GetDocumentPersistenceId(allDocs, t.Source),
                t =>
                {
                    var pathString = t.SourcePath.Path.ToString();
                    return allPaths
                        .First(p =>
                            p.DocumentType == t.Source.Type.Name &&
                            p.PathString == pathString).Id;
                });
        }
        public IndexDbRepo(DbContext dbc)
        {
            _dbc = dbc;
        }
    }
}
