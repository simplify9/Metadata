using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SW.Content;

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
            rhs = Expression.Constant(GetKeyValue(source), source.Key is ContentNumber? typeof(long?): typeof(string));
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
        
        DbDoc GetOrCreateDoc(DocumentToken token)
        {
            var docs = _dbc.ChangeTracker.Entries<DbDoc>()
                .Select(e => e.Entity)
                .Where(d => d.SourceType == token.Source.Type.Name);

            if (token.Source.Key is ContentNumber n)
            {
                var v = decimal.ToInt64(n.Value);
                docs = docs.Where(d => d.SourceIdNumber == v);
            }
            else if (token.Source.Key is ContentText t)
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
                    SourceIdNumber = token.Source.Key is ContentNumber num? (long?)decimal.ToInt64(num.Value): null,
                    SourceIdString = token.Source.Key is ContentText text? text.Value : null,
                    SourceType = token.Source.Type.Name,
                    Tokens = new List<DbDocToken>()
                };
                _dbc.Add(doc);
            }

            doc.LastIndexOn = DateTime.UtcNow;
            doc.BodyEncoding = "application/json";
            doc.BodyData = JsonUtil.Serialize(token.SourceData.ToJson());
            
            return doc;
        }

        DbDocSourcePath GetOrCreatePath(DocumentToken token)
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
            return path;
        }


        public async Task SaveTokens(DocumentToken[] tokens)
        {

            var docs = await BuildDocumentSetQueryable(tokens.Select(t => t.Source).Distinct())
                .Include(d => d.Tokens)
                .ToArrayAsync();
                //.Include(d => d.Tokens.Select(t => t.Path));

            // ensure paths
            var types = tokens.Select(t => t.Source.Type.Name).Distinct();
            var paths = await _dbc.Set<DbDocSourcePath>()
                .Where(p => types.Contains(p.DocumentType))
                .ToArrayAsync();
            
            foreach (var t in tokens)
            {
                
                var doc = GetOrCreateDoc(t);
                var path = GetOrCreatePath(t);
                var pathString = t.SourcePath.Path.ToString();
                var dbToken = doc.Tokens
                    .FirstOrDefault(tOld =>
                        tOld.Path.PathString == pathString &&
                        tOld.Offset == t.SourcePath.Offset);
                if (dbToken == null)
                {
                    dbToken = new DbDocToken
                    {
                        CreatedOn = DateTime.UtcNow,
                        Document = doc,
                        Path = path,
                        Offset = t.SourcePath.Offset
                    };

                    doc.Tokens.Add(dbToken);
                }
                
                dbToken.LastUpdatedOn = DateTime.UtcNow;

                dbToken.ValueAsAny = null;
                dbToken.ValueAsBoolean = null;
                dbToken.ValueAsDateTime = null;
                dbToken.ValueAsNumber = null;
                dbToken.ValueAsString = null;

                if (!(t.Normalized is ContentNull))
                {
                    dbToken.ValueAsAny = ((IContentPrimitive)t.Raw).CreateMatchKey();
                    if (t.Normalized is ContentNumber n) dbToken.ValueAsNumber = n.Value;
                    else if (t.Normalized is ContentDateTime dt) dbToken.ValueAsDateTime = dt.Value;
                    else if (t.Normalized is ContentBoolean b) dbToken.ValueAsBoolean = b.Value;
                    else if (t.Normalized is ContentText text) dbToken.ValueAsString = text.Value.ToLowerInvariant();
                }



            }

            await _dbc.SaveChangesAsync();
        }

        public async Task DeleteDocuments(DocumentSource[] sources)
        {
            var docsToDelete = BuildDocumentSetQueryable(sources).Include(d => d.Tokens);
            foreach (var doc in docsToDelete)
            {
                foreach (var docToken in doc.Tokens) doc.Tokens.Remove(docToken);
                _dbc.Remove(doc);
            }

            await _dbc.SaveChangesAsync();
        }

        public IndexDbRepo(DbContext dbc)
        {
            _dbc = dbc;
        }
    }
}
