using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Expressions
{
    public class CreateObjectExpression : IContentExpression, IEquatable<CreateObjectExpression>
    {
        public abstract class Element
        {

        }

        public class Attribute : Element
        {
            public string Name { get; set; }

            public IContentExpression Value { get; set; }

            public override string ToString()
            {
                return $"{Name}:{Value}";
            }

            public override bool Equals(object obj)
            {
                return obj is Attribute a && Name.Equals(a.Name) && Value.Equals(a.Value);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        public class Object : Element
        {
            public IContentExpression SubObject { get; set; }

            public override string ToString()
            {
                return $"...{SubObject}";
            }

            public override bool Equals(object obj)
            {
                return obj is Object o && SubObject.Equals(o.SubObject);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        public Element[] Elements { get; }

        //public IReadOnlyDictionary<string, IContentExpression> Attributes { get; }

        public CreateObjectExpression(IEnumerable<Element> elements)
        {
            Elements = elements?.ToArray() ?? throw new ArgumentNullException(nameof(elements));
        }

        public ExpressionIssue TryEvaluate(LexicalScope scope, out IContentNode result)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            result = null;

            var attributeMap = new Dictionary<string, object>();

            foreach (var e in Elements)
            {
                if (e is Attribute a)
                {
                    var issue = a.Value.TryEvaluate(scope, out IContentNode attributeValue);
                    if (issue != null) return issue;
                    attributeMap[a.Name] = attributeValue;
                }
                else if (e is Object o)
                {
                    var issue = o.SubObject.TryEvaluate(scope, out IContentNode objectValue);
                    
                    if (objectValue is ContentObject subObject)
                    {
                        foreach (var key in subObject.Keys)
                        {
                            subObject.TryEvaluate(ContentPath.Parse(key), out IContentNode value);
                            attributeMap[key] = subObject;
                        }
                    }
                    else if (objectValue is ContentNull)
                    {
                        // do nothing
                    }
                    else
                    {
                        return new ExpressionIssue("Cannot merge sub-object. The evaluation yielded a non-object result");
                    }
                }
            }

            result = new ContentObject(attributeMap, ContentFactory.Default);

            return null;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("{");
            sb.Append(string.Join(",", Elements.Select(i => i.ToString())));
            sb.Append("}");
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CreateObjectExpression);
        }

        public bool Equals(CreateObjectExpression other)
        {
            return other != null &&
                 Elements.SequenceEqual(other.Elements);
        }

        public override int GetHashCode()
        {
            return 1573927372 + EqualityComparer<Element[]>.Default.GetHashCode(Elements);
        }

        public static bool operator ==(CreateObjectExpression expression1, CreateObjectExpression expression2)
        {
            return EqualityComparer<CreateObjectExpression>.Default.Equals(expression1, expression2);
        }

        public static bool operator !=(CreateObjectExpression expression1, CreateObjectExpression expression2)
        {
            return !(expression1 == expression2);
        }
    }
}
