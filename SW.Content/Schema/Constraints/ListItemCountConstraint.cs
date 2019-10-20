using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Schema
{
    public class ListItemCountConstraint : IContentSchemaConstraint
    {
        public int? MinItemCount { get; private set; }

        public int? MaxItemCount { get; private set; }

        public ListItemCountConstraint(int? minItems, int? maxItems)
        {
            MinItemCount = minItems;
            MaxItemCount = maxItems;
        }

        public IEnumerable<SchemaIssue> FindIssues(IContentNode node)
        {
            var list = node as ContentList;
            var itemCount = list.Count();
            if ((MinItemCount != null && MinItemCount.Value > itemCount) ||
                (MaxItemCount != null && MaxItemCount.Value < itemCount))
            {
                var sb = new StringBuilder();
                sb.Append("Must have item count ");
                if (MinItemCount != null)
                {
                    sb.Append(">= ");
                    sb.Append(MinItemCount);
                }

                if (MaxItemCount != null)
                {
                    if (MinItemCount != null) sb.Append("and ");
                    sb.Append("<= ");
                    sb.Append(MaxItemCount);
                }

                sb.Append(", found ");
                sb.Append(itemCount);
                sb.Append(" items");

                yield return new SchemaIssue(ContentPath.Root, sb.ToString());
            }
        }
    }
}
