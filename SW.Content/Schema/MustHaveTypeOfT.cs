using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Schema
{
    public class MustHaveType<TContent> : IMust
        where TContent : IContentNode
    {
       
        public IEnumerable<ContentSchemaRule> Rules { get; }

        public MustHaveType(IEnumerable<ContentSchemaRule> rules)
        {
            Rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        public virtual IEnumerable<SchemaIssue> FindIssues(IContentNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (!(node is TContent))
            {
                yield return new SchemaIssue(ContentPath.Root,
                    $"Expected type {typeof(TContent)}, found {node.GetType()}");
            }
            else
            {
                //var ruleInput = new ContentObject(new[] { new KeyValuePair<string,object>("input", ) })
                foreach (var r in Rules)
                {
                    if (!r.Filter.IsMatch(node))
                    {
                        yield return new SchemaIssue(
                            ContentPath.Root,
                            $"Rule named '{r.Name}' did not match content node");
                    }
                }
            }
        }

        public virtual bool TryGetSchema(ContentPath path, out IMust schema)
        {
            schema = null;
            if (path.Length > 0) return false;
            schema = this;
            return true;
        }
    }
}
