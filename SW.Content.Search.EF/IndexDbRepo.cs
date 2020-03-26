using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace SW.Content.Search.EF
{
    public class IndexDbRepo : IIndexRepo
    {
        readonly DbContext _dbc;
        readonly IEntityType docTokenModel;
        readonly string schemaName;
        readonly ILogger<IndexDbRepo> _logger;


        // TODO support Guid keys !!
        public IndexDbRepo(DbContext dbc, ILogger<IndexDbRepo> logger)
        {
            _dbc = dbc;
            docTokenModel = _dbc.Model.FindRuntimeEntityType(typeof(DbDocToken));
            schemaName = docTokenModel.GetSchema();
            _logger = logger;

        }

        static string GetKeyFieldName(DocumentSource source)
        {
            if (source.Key is ContentNumber) return nameof(DbDoc.SourceIdNumber);
            if (source.Key is ContentText) return nameof(DbDoc.SourceIdString);
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

        static DbDoc FindDocument(IEnumerable<DbDoc> docs, DocumentSource source)
        {
            var docsWithType = docs.Where(d => d.SourceType == source.Type.Name);
            if (source.Key is ContentNumber n)
            {
                var v = decimal.ToInt64(n.Value);
                docsWithType = docsWithType.Where(d => d.SourceIdNumber == v);
            }
            else if (source.Key is ContentText t)
            {
                docsWithType = docsWithType.Where(d => d.SourceIdString == t.Value);
            }
            else
            {
                throw new NotSupportedException("Document has a key of type that is not supported");
            }

            return docsWithType.FirstOrDefault();
        }

        void AddOrUpdateDocument(DocumentSource source, object sourceData)
        {
            var docs = _dbc.ChangeTracker.Entries<DbDoc>().Select(d => d.Entity);
            var doc = FindDocument(docs, source);
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
                _dbc.Set<DbDoc>().Add(doc);
            }

            doc.LastIndexOn = DateTime.UtcNow;
            doc.BodyEncoding = "application/json";
            doc.BodyData = JsonUtil.Serialize(sourceData);
        }

        async Task SaveTokens(DocumentToken[] newTokens, 
            DocumentToken[] oldTokens, 
            IEnumerable<DbDoc> dbDocs,
            IEnumerable<DbDocSourcePath> dbPaths)
        {
            // compare old tokens with new tokens

            var addedTokens = newTokens
                .Where(t => !oldTokens
                    .Any(dbt => 
                        t.SourcePath.Equals(dbt.SourcePath) && 
                        t.Source.Equals(dbt.Source)));

            var updatedTokens = newTokens
                .Where(t => oldTokens
                    .Any(dbt => 
                        t.SourcePath.Equals(dbt.SourcePath) && 
                        t.Source.Equals(dbt.Source) && 
                        !t.Raw.Equals(dbt.Raw)));

            var deletedTokens = oldTokens
                .Where(t => !newTokens
                    .Any(nt => 
                        nt.Source.Equals(t.Source) && 
                        t.SourcePath.Equals(nt.SourcePath)));
            
            // add is 0 ,1 is update and delete is 2
            var addAndMergeUpdatesAndDeletes = addedTokens
                .Select(a => new Tuple<int, DocumentToken>(0, a))
                .Concat(updatedTokens.Select(u => new Tuple<int, DocumentToken>(1, u))
                .Concat(deletedTokens.Select(d => new Tuple<int, DocumentToken>(2, d)))).ToArray();

            var addAndUpdateAndDeletebulks = addAndMergeUpdatesAndDeletes.ToBatch(25, t => t);

            // build SQL query batches

            var conn = _dbc.Database.GetDbConnection();
            var schemaNamePrefix = conn.ToString() == "Microsoft.Data.Sqlite.SqliteConnection" ? string.Empty : $"[{schemaName}].";

            var now = DateTime.UtcNow;
            
            foreach (var chg in addAndUpdateAndDeletebulks)
            {
                var stringBuilder = new StringBuilderHelper();
                foreach (var tuple in chg)
                {
                    var token = tuple.Item2;

                    var docId = FindDocument(dbDocs, token.Source)?.Id ?? 0;

                    var pathString = token.SourcePath.Path.ToString();
                    var pathId = dbPaths
                        .First(p =>
                            p.DocumentType == token.Source.Type.Name &&
                            p.PathString == pathString).Id;

                    var offset = token.SourcePath.Offset;
                    var valueAsAny = ((IContentPrimitive)token.Raw).CreateMatchKey();

                    if (tuple.Item1 == 0)
                    {
                        stringBuilder.Append($@"
                INSERT INTO {schemaNamePrefix}[DocTokens]
                            ([DocumentId],[PathId],[Offset],[CreatedOn],[LastUpdatedOn],[ValueAsAny])
                        VALUES
                            ( ? , ? , ? , ? , ? , ?)", docId, pathId, offset, now, now, valueAsAny);
                    }
                    else if (tuple.Item1 == 1)
                    {
                        stringBuilder.Append($@"
                UPDATE {schemaNamePrefix}[DocTokens]
                SET
                    [LastUpdatedOn] = ?,ValueAsAny = ?
                WHERE [DocumentId] = ? and [PathId] = ? and [Offset] = ?",
                                now,
                                valueAsAny, docId, pathId, offset);
                    }
                    else if (tuple.Item1 == 2)
                    {
                        if (pathId > 0)
                        {
                            stringBuilder.Append($@"
                    DELETE FROM {schemaNamePrefix}[DocTokens]
                        WHERE [DocumentId] = ? and [PathId] = ? and [Offset] = ? ", docId, pathId, offset);
                        }
                    }
                }

                await stringBuilder.ExecuteSqlCommand(_dbc);
            }
        }

        public async Task DeleteDocuments(DocumentSource[] sources)
        {
            var docsToDelete = await BuildDocumentSetQueryable(sources).Include(d => d.Tokens).ToArrayAsync();
           
            foreach (var doc in docsToDelete)
            {
                var tokens = doc.Tokens.ToArray();
                foreach (var docToken in tokens) doc.Tokens.Remove(docToken);
                _dbc.Remove(doc);
            }

            await _dbc.SaveChangesAsync();
        }

        public async Task UpdateDocuments(Document[] docs)
        {
            using (var trx = await _dbc.Database.BeginTransactionAsync())
            {
                try
                {
                    // load docs
                    var dbDocs = await BuildDocumentSetQueryable(docs.Select(d => d.Source)).ToArrayAsync();

                    // ensure paths
                    var types = docs.Select(t => t.Source.Type.Name).Distinct();
                    var paths = await _dbc.Set<DbDocSourcePath>()
                        .Where(p => types.Contains(p.DocumentType))
                        .ToArrayAsync();

                    // get old token pairs from db doc
                    var oldTokens = dbDocs.SelectMany(doc =>
                       TokenHelper.GetTokens(
                           new DocumentSource(
                               new DocumentType(Type.GetType(doc.SourceType)),
                               ContentFactory.Default.CreateFrom(doc.SourceIdString)),
                           ContentFactory.Default.CreateFrom(JsonUtil.Deserialize<JToken>(doc.BodyData)).ToJson())
                        ).ToArray();

                    // add / update documents
                    foreach (var doc in docs) AddOrUpdateDocument(doc.Source, doc.Data);

                    // tokenize incoming documents
                    var newTokens = docs.SelectMany(u => TokenHelper.GetTokens(u.Source, u.Data)).ToArray();

                    // add new paths
                    var newPaths = newTokens
                        .Select(t => (path: t.SourcePath.Path.ToString(), docType: t.Source.Type.Name))
                        .Where(t => !paths
                            .Any(p => p.PathString == t.path && p.DocumentType == t.docType))
                            .Distinct()
                            .Select(t => new DbDocSourcePath
                            {
                                CreatedOn = DateTime.UtcNow,
                                DocumentType = t.docType,
                                PathString = t.path
                            });
                    _dbc.Set<DbDocSourcePath>().AddRange(newPaths);
                    
                    // persist docs and paths changes to generate sequence IDs
                    await _dbc.SaveChangesAsync();

                    dbDocs = _dbc.ChangeTracker.Entries<DbDoc>().Select(e => e.Entity).ToArray();
                    paths = _dbc.ChangeTracker.Entries<DbDocSourcePath>().Select(e => e.Entity).ToArray();

                    await SaveTokens(newTokens, oldTokens, dbDocs, paths);

                    await trx.CommitAsync();
                }
                catch (Exception)
                {
                    await trx.RollbackAsync();
                    throw;
                }
            }
        }
 

    }
}
