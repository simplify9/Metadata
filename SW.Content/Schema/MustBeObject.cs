using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Schema
{
    public class MustBeObject : MustHaveType<ContentObject>
    {
        public IMust ItemType { get; } 

        public ContentProperty[] Properties { get;  }

        public MustBeObject(IEnumerable<ContentProperty> properties, IEnumerable<ContentSchemaRule> rules)
            : base(rules)
        {
            Properties = properties?.ToArray() ?? throw new ArgumentNullException(nameof(properties));
        }

        public MustBeObject(IMust itemType, IEnumerable<ContentSchemaRule> rules)
            : base(rules)
        {
            Properties = null;
            ItemType = itemType ?? throw new ArgumentNullException(nameof(itemType));
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
                if (Properties != null)
                {
                    foreach (var prop in Properties)
                    {
                        var path = new ContentPath(new string[] { prop.Key });

                        var exists = node.TryEvaluate(path, out IContentNode result);

                        if (exists)
                        {
                            var fieldIssues = prop.Value.FindIssues(result);
                            foreach (var issue in fieldIssues)
                            {
                                yield return new SchemaIssue(
                                    path.Append(issue.SubjectPath),
                                    issue.Error);
                            }
                        }
                        else if (prop.IsRequired)
                        {
                            yield return new SchemaIssue(path, $"required but missing");
                        }
                    }
                }
                else
                {
                    if (node is ContentObject objectNode)
                    {
                        foreach (var key in objectNode.Keys)
                        {
                            var path = ContentPath.Parse(key);
                            node.TryEvaluate(path, out IContentNode result);
                            
                            var fieldIssues = ItemType.FindIssues(result);
                            foreach (var issue in fieldIssues)
                            {
                                yield return new SchemaIssue(
                                    path.Append(issue.SubjectPath),
                                    issue.Error);
                            }
                        }
                    }
                    
                }
            }

            yield break;
        }

    }
}
