using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Schema
{
    public class MustBeList : MustHaveType<ContentList>
    {
        public IMust Item { get; private set; }

        public int? MinItemCount { get; private set; }

        public int? MaxItemCount { get; private set; }

        public MustBeList(IMust item, int? minCount, int? MaxCount)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));
            MinItemCount = minCount;
            MaxItemCount = MaxCount;
        }

        public override IEnumerable<SchemaIssue> FindIssues(IContentNode node)
        {
            var issues = base.FindIssues(node);

            if (issues.Any())
            {
                foreach (var i in issues) yield return i;
            }
            else
            {
                var list = node as ContentList;

                var itemCount = list.Items.Count();
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
                }
                else
                {
                    var idx = 0;
                    foreach (var item in list.Items)
                    {
                        var index = new ContentPath(new string[] { $"[{idx}]" });
                        issues = Item.FindIssues(item);
                        foreach (var issue in issues)
                        {
                            yield return new SchemaIssue(
                                index.Append(issue.SubjectPath), 
                                issue.Error);
                        }
                        ++idx;
                    }
                }
            }
        }
    }
}
