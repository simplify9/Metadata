using SW.Content;
using SW.Content.Search.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SW.Content.Search
{


    public class IndexService
    {
        readonly IIndexRepo _repo;
        
        
        public UpdateIndexCommand CreateUpdateCommand(object key, object source)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (source == null) throw new ArgumentNullException(nameof(source));

            var docType = new DocumentType(source.GetType());
            var docSource = new DocumentSource(docType, ContentFactory.Default.CreateFrom(key));
            var sourceData = ContentFactory.Default.CreateFrom(source);
            if (sourceData == null)
            {
                throw new InvalidOperationException(
                    $"Objects of type '{source.GetType()}' are not supported");
            }

            return new UpdateIndexCommand(docSource, sourceData);
        }



        public DropIndexCommand CreateDropCommand(object key, object source)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (source == null) throw new ArgumentNullException(nameof(source));

            var docType = new DocumentType(source.GetType());
            var docSource = new DocumentSource(docType, ContentFactory.Default.CreateFrom(key));
            return new DropIndexCommand(docSource);
        }

        IEnumerable<ContentPathValue> TokenFilter(IEnumerable<ContentPathValue> source)
        {
            foreach (var pair in source)
            {
                var lastNode = pair.Path.LastOrDefault();
                if (lastNode != null && lastNode is ContentPath.ListIndex)
                {
                    yield return new ContentPathValue(
                        pair.Path.Sub(0, pair.Path.Length - 1),
                            pair.Value);
                }

                yield return pair;
            }
        }

        IEnumerable<DocumentToken> GetTokens(DocumentSource source, IContentNode sourceData)
        {
            var pairs = sourceData.Visit().Where(p => p.Value is IContentPrimitive);
            ContentPath lastPath = null;
            var offset = 0;
            // filter pairs
            var filteredPairs = TokenFilter(pairs)
                // order by path
                .OrderBy(p => p.Path.ToString());

            // build tokens from pairs
            foreach (var pair in filteredPairs)
            {
                // assign incremental offsets for similar paths
                offset = pair.Path.Equals(lastPath) ? offset + 1 : 0;
                var path = new DocumentSourcePath(pair.Path, offset);
                lastPath = pair.Path;
                yield return new DocumentToken(source, sourceData, path, pair.Value, pair.Value);
            }
        }
        
        public async Task Handle(params IIndexCommand[] commands)
        {
            // update document(s) tokens

            var tokens = commands
                .OfType<UpdateIndexCommand>()
                .SelectMany(u => GetTokens(u.Source, u.SourceData));
            
            if (tokens.Any()) await _repo.SaveTokens(tokens.ToArray());

            // drop documents

            var deletes = commands.OfType<DropIndexCommand>().Select(cmd => cmd.Source);

            if (deletes.Any()) await _repo.DeleteDocuments(deletes.ToArray());
            
        }
        
        public IndexService(IIndexRepo repo)
        {
            _repo = repo;
        }
    }
}
