using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SW.Content.Schema
{
    public class MustMatchRegex : MustHaveType<ContentText>
    {
        readonly Regex _regex;
        
        public MustMatchRegex(string regex)
        {
            if (regex == null)
            {
                throw new ArgumentNullException(nameof(regex));
            }

            _regex = new Regex(regex, RegexOptions.Compiled);
        }

        public override IEnumerable<SchemaIssue> FindIssues(IContentNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            var issues = base.FindIssues(node);

            if (issues.Any())
            {
                foreach (var i in issues) yield return i;
            }
            else
            {
                var value = (node as ContentText).Value;
                if (!_regex.IsMatch(value))
                {
                    yield return new SchemaIssue(ContentPath.Root(),
                        $"Value '{value}' has invalid format");
                }
            }

            yield break;
        }
    }
}
