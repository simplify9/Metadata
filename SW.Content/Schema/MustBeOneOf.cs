using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Schema
{
    public class MustBeOneOf : TypeDef<IContentNode>
    {
        
        public IEnumerable<ITypeDef> Options { get; }

        public MustBeOneOf(IEnumerable<ITypeDef> options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public override IEnumerable<SchemaIssue> FindIssues(IContentNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            var issues = base.FindIssues(node);
            if (issues.Any())
            {
                foreach (var i in issues)
                {
                    yield return i;
                }
            }
            else
            {
                foreach (var option in Options)
                {
                    issues = option.FindIssues(node);
                    if (!issues.Any())
                    {
                        yield break;
                    }
                }

                yield return new SchemaIssue(ContentPath.Root,
                    $"Value did not match any of the following: {string.Join(",", Options)}");

                yield break;
            }
            
        }
        
    }
}
