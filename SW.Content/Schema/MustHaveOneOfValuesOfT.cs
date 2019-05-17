using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Schema
{
    public class MustHaveOneOfValues<TContent> : MustHaveType<TContent>
        where TContent : IContentNode
    {
        public TContent[] Values { get; }

        public MustHaveOneOfValues(IEnumerable<TContent> values) 
        {
            Values = values?.ToArray() ?? throw new ArgumentNullException(nameof(values));
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
                if (!Values.Contains((TContent)node))
                {
                    yield return new SchemaIssue(ContentPath.Root(),
                        $"Allowed values are {string.Join(",", Values)}, found {node}");
                }
            }

            yield break;
        }
    }
}
