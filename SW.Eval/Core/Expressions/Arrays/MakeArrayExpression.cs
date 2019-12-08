using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval
{
    public class MakeArrayExpression : IEvalExpression, IEquatable<MakeArrayExpression>
    {
        public abstract class Element
        {
            public IEvalExpression Value { get; set; }
        }

        public class ListItem : Element
        {
            public override string ToString() => Value.ToString();
            
            public override bool Equals(object obj) => obj is ListItem li && Value.Equals(li.Value);
            
            public override int GetHashCode() => Value.GetHashCode();
        }

        public class List : Element
        {
            public override string ToString() => $"...{Value}";
            
            public override bool Equals(object obj) => obj is List l && Value.Equals(l.Value);
            
            public override int GetHashCode() => Value.GetHashCode();
        }
        
        public IEnumerable<Element> Items { get; }

        public MakeArrayExpression(IEnumerable<Element> items)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("[");
            sb.Append(string.Join(",", Items.Select(i => i.ToString())));
            sb.Append("]");
            return sb.ToString();
        }

        public override bool Equals(object obj) => Equals(obj as MakeArrayExpression);
        
        public bool Equals(MakeArrayExpression other) 
            => other != null &&  Items.SequenceEqual(other.Items);
        
        public override int GetHashCode()
        {
            return -604923257 + EqualityComparer<IEnumerable<Element>>.Default.GetHashCode(Items);
        }

        public IEnumerable<IEvalExpression> GetChildren() => Items.Select(item => item.Value);

        public IEvalState ComputeState(EvalContext ctx)
            => ctx.Apply(args =>
            {
                // result array elements
                var payloadItems = Items

                    // ignore empty yields
                    .Where(i => 
                        !(args[i.Value] is INoPayload) && 
                        !(i is List && args[i.Value] is INull))

                    // flatten contents
                    .SelectMany(i =>
                        i is List list
                            ? args[i.Value] is ISet set
                                ? set.Items
                                : new[] { new PayloadError($"Cannot concat a non-array into an array") }
                            : new[] { args[i.Value] });
                
                // result array
                return new EvalComplete(PayloadArray.Combine(payloadItems));
            });
        

        public static bool operator ==(MakeArrayExpression expression1, MakeArrayExpression expression2)
        {
            return EqualityComparer<MakeArrayExpression>.Default.Equals(expression1, expression2);
        }

        public static bool operator !=(MakeArrayExpression expression1, MakeArrayExpression expression2)
        {
            return !(expression1 == expression2);
        }
    }
}
