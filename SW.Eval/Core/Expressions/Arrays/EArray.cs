using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval
{
    public class EArray : IEvalExpression, IEquatable<EArray>
    {
        public abstract class Element
        {
            public IEvalExpression Value { get; set; }

            public abstract IEnumerable<IPayload> CreateItems(IPayload p);
        }

        public class ListItem : Element
        {
            public override string ToString() => Value.ToString();
            
            public override bool Equals(object obj) => obj is ListItem li && Value.Equals(li.Value);
            
            public override int GetHashCode() => Value.GetHashCode();

            public override IEnumerable<IPayload> CreateItems(IPayload p)
            {
                if (!(p is INoPayload))
                {
                    yield return p;
                }
            }
        }

        public class List : Element
        {
            public override string ToString() => $"...{Value}";
            
            public override bool Equals(object obj) => obj is List l && Value.Equals(l.Value);
            
            public override int GetHashCode() => Value.GetHashCode();

            public override IEnumerable<IPayload> CreateItems(IPayload p)
            {
                return p is ISet set
                    ? set.Items
                    : Array.Empty<IPayload>();
            }
        }
        
        public IEnumerable<Element> Items { get; }

        public EArray(IEnumerable<Element> items)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("[");
            sb.Append(string.Join(", ", Items.Select(i => i.ToString())));
            sb.Append("]");
            return sb.ToString();
        }

        public override bool Equals(object obj) => Equals(obj as EArray);
        
        public bool Equals(EArray other) 
            => other != null &&  Items.SequenceEqual(other.Items);
        
        public override int GetHashCode()
        {
            return -604923257 + EqualityComparer<IEnumerable<Element>>.Default.GetHashCode(Items);
        }

        public IEnumerable<EvalArg> GetArgs() => 
            Items.Select((i,idx) => 
                i is ListItem
                    ? new EvalArg($"frag{idx}", i.Value)
                    : new EvalArg($"frag{idx}", i.Value, 
                        p => p is INull || p is INoPayload || p is ISet,
                            "Given type cannot be flattened as an array"));
        
        public EvalStateMapper GetMapper() =>
            (ctx, args) =>
            {
                var pairs = Enumerable.Zip(Items, args, (Fragment, Value) => (Fragment, Value))
                    .SelectMany(i => i.Fragment.CreateItems(i.Value));

                return new EvalComplete(PayloadArray.Combine(pairs));
            };

        public static bool operator ==(EArray expression1, EArray expression2)
        {
            return EqualityComparer<EArray>.Default.Equals(expression1, expression2);
        }

        public static bool operator !=(EArray expression1, EArray expression2)
        {
            return !(expression1 == expression2);
        }
    }
}
