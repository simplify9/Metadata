using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Describe.Models
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

        public ExpressionIssue TryEvaluate(IPayload input, out IPayload result)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            
            result = null;

            var attributeMap = new Dictionary<string, IPayload>();

            foreach (var e in Elements)
            {
                if (e is Attribute a)
                {
                    var issue = a.Value.TryEvaluate(input, out IPayload attributeValue);
                    if (issue != null) return issue;
                    attributeMap[a.Name] = attributeValue;
                }
                else if (e is Object o)
                {
                    var issue = o.SubObject.TryEvaluate(input, out IPayload objectValue);
                    
                    if (objectValue is EObject subObject)
                    {
                        //foreach (var key in subObject.Keys)
                        //{
                        //    subObject.TryEvaluate(EPath.Root.Append(key), out IPayload value);
                        //    attributeMap[key] = subObject;
                        //}
                    }
                    else if (objectValue is ENull)
                    {
                        // do nothing
                    }
                    else
                    {
                        return new ExpressionIssue("Cannot merge sub-object. The evaluation yielded a non-object result");
                    }
                }
            }

            result = new EObject(attributeMap);

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
