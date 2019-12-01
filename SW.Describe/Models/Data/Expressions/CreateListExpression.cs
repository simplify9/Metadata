using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Describe.Models
{
    public class CreateListExpression : IContentExpression, IEquatable<CreateListExpression>
    {
        public abstract class Element
        {
            public IContentExpression Value { get; set; }
        }

        public class ListItem : Element
        {
            public override string ToString()
            {
                return Value.ToString();
            }

            public override bool Equals(object obj)
            {
                return obj is ListItem li && Value.Equals(li.Value);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        public class List : Element
        {
            public override string ToString()
            {
                return $"...{Value}";
            }

            public override bool Equals(object obj)
            {
                return obj is List l && Value.Equals(l.Value);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        public IEnumerable<Element> Items { get; }

        public CreateListExpression(IEnumerable<Element> items)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }

        public ExpressionIssue TryEvaluate(IPayload input, out IPayload result)
        {
            var list = new List<IPayload>();
            foreach (var e in Items)
            {
                var issue = e.Value.TryEvaluate(input, out IPayload item);
                if (issue != null)
                {
                    result = null;
                    return issue;
                }
                
                if (e is ListItem a)
                {
                    list.Add(item);
                }
                else if (e is List l)
                {
                    if (item is ESet subList)
                    {
                        list.AddRange(subList);
                    }
                    else if (item is ENull)
                    {
                        // do nothing
                    }
                    else
                    {
                        result = null;
                        return new ExpressionIssue("Cannot merge a non-list into a list");
                    }
                }

                if (item is ESet listItem)
                {
                    list.AddRange(listItem);
                }
                else list.Add(item);
            }
            result = new ESet(list);

            return null;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("[");
            sb.Append(string.Join(",", Items.Select(i => i.ToString())));
            sb.Append("]");
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CreateListExpression);
        }

        public bool Equals(CreateListExpression other)
        {
            
            return other != null &&  Items.SequenceEqual(other.Items);
        }

        public override int GetHashCode()
        {
            return -604923257 + EqualityComparer<IEnumerable<Element>>.Default.GetHashCode(Items);
        }

        public static bool operator ==(CreateListExpression expression1, CreateListExpression expression2)
        {
            return EqualityComparer<CreateListExpression>.Default.Equals(expression1, expression2);
        }

        public static bool operator !=(CreateListExpression expression1, CreateListExpression expression2)
        {
            return !(expression1 == expression2);
        }
    }
}
