using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Schema
{
    public class ListItemTypeConstraint : IContentSchemaConstraint
    {
        public ITypeDef Item { get; private set; }

        public ListItemTypeConstraint(ITypeDef item)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));
        }

        public IEnumerable<SchemaIssue> FindIssues(IContentNode node)
        {
            var list = node as ContentList;
            var idx = 0;
            foreach (var item in list)
            {
                var index = ContentPath.Root.Append(idx);
                var issues = Item.FindIssues(item);
                foreach (var issue in issues)
                {
                    yield return new SchemaIssue(index.Append(issue.SubjectPath), issue.Error);
                }
                ++idx;
            }
        }
    }
}
