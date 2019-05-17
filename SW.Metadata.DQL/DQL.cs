using SW.Metadata.Core;
using SW.Metadata.DQL.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Metadata.DQL
{
    public static class DQL
    {
        static readonly DQLParser _parser = new DQLParser(new Tokenizer());

        public static DQLParser.Issue[] TryParse(string dql, out IContentFilter result)
        {
            return _parser.TryParse(dql, out result);
        }

        public static IContentFilter Parse(string dql)
        {
            if (dql == null) throw new ArgumentNullException(nameof(dql));

            var issues = _parser.TryParse(dql, out IContentFilter result);

            if (issues.Any()) throw new FormatException($"The following DQL was invalid: {dql}");

            return result;
        }


    }
}
