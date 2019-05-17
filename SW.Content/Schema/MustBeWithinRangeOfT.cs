using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Schema
{
    public class MustBeWithinRange<TContent> : MustHaveType<TContent>
        where TContent : IContentNode, IContentPrimitive
    {
        public bool MinValueInclusive { get; private set; }
        public TContent MinValue { get; private set; }

        public bool MaxValueInclusive { get; private set; }
        public TContent MaxValue { get; private set; }
        
        public MustBeWithinRange(
            TContent min, bool minInclusive,
            TContent max, bool maxInclusive)
        {
            MinValue = min;
            MinValueInclusive = minInclusive;
            MaxValue = max;
            MaxValueInclusive = maxInclusive;
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
                var pass = true;
                if (MinValue != null)
                {
                    var result = node.CompareWith(MinValue);
                    if (result != ComparisonResult.GreaterThan)
                    {
                        if (!MinValueInclusive || result != ComparisonResult.EqualTo)
                        {
                            pass = false;
                        }
                    }
                }

                if (pass)
                {
                    if (MaxValue != null)
                    {
                        var result = node.CompareWith(MaxValue);
                        if (result != ComparisonResult.LessThan)
                        {
                            if (!MaxValueInclusive || result != ComparisonResult.EqualTo)
                            {
                                pass = false;
                            }
                        }
                    }
                }
                
                if (!pass)
                {
                    var sb = new StringBuilder();
                    sb.Append("Expected a value ");
                    if (MinValue != null)
                    {
                        sb.Append("less than ");
                        if (MinValueInclusive) sb.Append("or equal to ");
                        sb.Append(MinValue);
                    }

                    if (MaxValue != null)
                    {
                        if (MinValue != null) sb.Append(" and ");
                        sb.Append("greater than ");
                        if (MaxValueInclusive) sb.Append("or equal to ");
                        sb.Append(MaxValue);
                    }

                    sb.Append(", found ");
                    sb.Append(node);

                    yield return new SchemaIssue(ContentPath.Root(), sb.ToString());
                }
            }

            yield break;
        }
    }
}
