using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Schema
{
    public class MustHaveType<TContent> : IMust
        where TContent : IContentNode
    {
        public virtual IEnumerable<SchemaIssue> FindIssues(IContentNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (!(node is TContent))
            {
                yield return new SchemaIssue(ContentPath.Root(),
                    $"Expected type {typeof(TContent)}, found {node.GetType()}");
            }
        }
    }
}
