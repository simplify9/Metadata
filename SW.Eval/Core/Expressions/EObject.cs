using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval
{
    public class EObject : IEvalExpression, IEquatable<EObject>
    {
        public abstract class Element
        {
            public IEvalExpression Value { get; set; }

            public abstract IEnumerable<KeyValuePair<PayloadPath, IPayload>> CreatePairs(IPayload p);
        }

        public class Attribute : Element
        {
            public string Name { get; set; }

            public override IEnumerable<KeyValuePair<PayloadPath, IPayload>> CreatePairs(IPayload p)
            {
                if (!(p is INoPayload))
                {
                    yield return p.MakePair(PayloadPath.Root.Append(Name));
                }
            }

            public override string ToString() => $"{Name}:{Value}";
            
            public override bool Equals(object obj) 
                => obj is Attribute a && 
                    Name.Equals(a.Name) && 
                    Value.Equals(a.Value);
            
            public override int GetHashCode() => Value.GetHashCode();

            
        }

        public class Object : Element
        {
            public override IEnumerable<KeyValuePair<PayloadPath, IPayload>> CreatePairs(IPayload p)
            {
                return p is IObject o
                    ? (IEnumerable<KeyValuePair<PayloadPath, IPayload>>)o
                    : Array.Empty<KeyValuePair<PayloadPath, IPayload>>();
            }

            public override string ToString() => $"...{Value}";
            
            public override bool Equals(object obj) => obj is Object o && Value.Equals(o.Value);
            
            public override int GetHashCode() => Value.GetHashCode();
        }

        

        public Element[] Elements { get; }
        
        public EObject(IEnumerable<Element> elements)
        {
            Elements = elements?.ToArray() ?? throw new ArgumentNullException(nameof(elements));
        }
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("{");
            sb.Append(string.Join(",", Elements.Select(i => i.ToString())));
            sb.Append("}");
            return sb.ToString();
        }

        public override bool Equals(object obj) => Equals(obj as EObject);
        
        public bool Equals(EObject other) 
            => other != null &&
                 Elements.SequenceEqual(other.Elements);
        

        public override int GetHashCode()
        {
            return 1573927372 + EqualityComparer<Element[]>.Default.GetHashCode(Elements);
        }

        public IEnumerable<EvalArg> GetArgs() =>
            Elements.Select((i, idx) =>
                i is Attribute
                    ? new EvalArg($"frag{idx}", i.Value)
                    : new EvalArg($"frag{idx}", i.Value,
                        p => p is INull || p is INoPayload || p is IObject,
                            "Given type cannot be flattened as an object"));

        public EvalStateMapper GetMapper() =>
            (ctx, args) =>
            {
                var pairs = Enumerable.Zip(Elements, args, (Fragment, Value) => (Fragment, Value))
                    .SelectMany(i => i.Fragment.CreatePairs(i.Value));

                return new EvalComplete(PayloadObject.Combine(pairs));
            }; 
        
        public static bool operator ==(EObject expression1, EObject expression2)
        {
            return EqualityComparer<EObject>.Default.Equals(expression1, expression2);
        }

        public static bool operator !=(EObject expression1, EObject expression2)
        {
            return !(expression1 == expression2);
        }
    }
}
