using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SW.Content.Schema
{
    public class RegexConstraint : IContentSchemaConstraint
    {
        public Regex Regex { get; private set; }

        public RegexConstraint(string regex)
        {
            if (regex == null)
            {
                throw new ArgumentNullException(nameof(regex));
            }

            Regex = new Regex(regex, RegexOptions.Compiled);
        }

        public IEnumerable<SchemaIssue> FindIssues(IContentNode node)
        {
            if (node is ContentText text)
            {
                if (!Regex.IsMatch(text.Value))
                {
                    yield return new SchemaIssue(ContentPath.Root, $"value '{text.Value}' did not match required format");
                }
            }
        }
    }
}
