using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval
{
    public class MakeObjectExpression : IEvalExpression, IEquatable<MakeObjectExpression>
    {
        public abstract class Element
        {
            public IEvalExpression Value { get; set; }
        }

        public class Attribute : Element
        {
            public string Name { get; set; }
            
            public override string ToString() => $"{Name}:{Value}";
            
            public override bool Equals(object obj) 
                => obj is Attribute a && 
                    Name.Equals(a.Name) && 
                    Value.Equals(a.Value);
            
            public override int GetHashCode() => Value.GetHashCode();
        }

        public class Object : Element
        {
            public override string ToString() => $"...{Value}";
            
            public override bool Equals(object obj) => obj is Object o && Value.Equals(o.Value);
            
            public override int GetHashCode() => Value.GetHashCode();
        }

        public Element[] Elements { get; }
        
        public MakeObjectExpression(IEnumerable<Element> elements)
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

        public override bool Equals(object obj) => Equals(obj as MakeObjectExpression);
        
        public bool Equals(MakeObjectExpression other) 
            => other != null &&
                 Elements.SequenceEqual(other.Elements);
        

        public override int GetHashCode()
        {
            return 1573927372 + EqualityComparer<Element[]>.Default.GetHashCode(Elements);
        }

        public IEnumerable<IEvalExpression> GetChildren() => Elements.Select(e => e.Value);

        public IEvalState ComputeState(EvalContext ctx)
            => ctx.Apply(args =>
            {
                // result array elements
                var payloadItems = Elements

                    // ignore empty yields
                    .Where(i =>
                        !(args[i.Value] is INoPayload) &&
                        !(i is Object && args[i.Value] is INull))

                    // flatten contents
                    .SelectMany(i =>
                        i is Object objFragment
                            ? (args[i.Value] is IObject obj
                                ? (obj as IEnumerable<KeyValuePair<PayloadPath,IPayload>>)
                                : new[] {
                                    new KeyValuePair<PayloadPath, IPayload>(PayloadPath.Root, new PayloadError($"Cannot concat a non-array into an array"))
                                })
                            : new[] { new KeyValuePair<PayloadPath, IPayload>(PayloadPath.Root.Append(((Attribute)i).Name), args[i.Value]) });

                // result array
                return new EvalComplete(PayloadObject.Combine(payloadItems));
            });

        public static bool operator ==(MakeObjectExpression expression1, MakeObjectExpression expression2)
        {
            return EqualityComparer<MakeObjectExpression>.Default.Equals(expression1, expression2);
        }

        public static bool operator !=(MakeObjectExpression expression1, MakeObjectExpression expression2)
        {
            return !(expression1 == expression2);
        }
    }
}
