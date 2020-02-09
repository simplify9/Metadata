using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Search
{
    public static class TokenHelper
    {
        static IEnumerable<ContentPathValue> TokenizePairs(IEnumerable<ContentPathValue> source)
        {
            foreach (var pair in source)
            {
                if (pair.Value.ContentType() == ContentType.Text)
                {
                    var tokens = (pair.Value as ContentText).Value.Split(' ');

                    foreach (var t in tokens)
                    {
                        if (t.Length < 200 && t != string.Empty)
                        {
                            yield return new ContentPathValue(pair.Path, new ContentText(t));
                        }
                    }
                }
                else yield return pair;
            }
        }

        static IEnumerable<ContentPathValue> NullFilter(IEnumerable<ContentPathValue> source)
        {
            foreach (var pair in source)
            {
                if (!(pair.Value is ContentNull))
                {
                    yield return pair;
                }
            }
        }

        static IEnumerable<ContentPathValue> TokenFilter(IEnumerable<ContentPathValue> source)
        {
            foreach (var pair in source)
            {
                var lastNode = pair.Path.LastOrDefault();
                if (lastNode != null && lastNode is ContentPath.AnyListIndex)
                {
                    yield return new ContentPathValue(
                        pair.Path.Sub(0, pair.Path.Length - 1),
                            pair.Value);
                }
                else yield return pair;
            }
        }

        public static IEnumerable<DocumentToken> GetTokens(DocumentSource source, object sourceData)
        {
            var visit = ContentFactory.Default.CreateFrom(sourceData).Visit();
            var visitWhere = ContentFactory.Default.CreateFrom(sourceData).Visit().Where(p => p.Value is IContentPrimitive);

            var pairs = ContentFactory.Default.CreateFrom(sourceData).Visit()
                .Where(p => p.Value is IContentPrimitive)
                .Select(p => new ContentPathValue(
                    ContentPath.Root
                        .Append(p.Path
                            .Select(n =>
                                n is ContentPath.ListIndex ? ContentPath.AnyIndex : n)), p.Value));
            ContentPath lastPath = null;
            var offset = 0;
            // filter pairs
            var filteredPairs = NullFilter(TokenFilter(TokenizePairs(pairs)))
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
    }
}
