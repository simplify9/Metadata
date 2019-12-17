using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public class EConstant : IEvalExpression, IEquatable<EConstant>
    {
        public static EConstant From<T>(T value)
        {
            return new EConstant(new PayloadPrimitive<T>(value));
        }

        public IPayload Constant { get; }

        public EConstant(IPayload constant)
        {
            Constant = constant ?? throw new ArgumentNullException(nameof(constant));
        }
        

        public override string ToString() => Constant.ToString();
        
        public override bool Equals(object obj) => Equals(obj as EConstant);
        
        public bool Equals(EConstant other) => other != null && Constant.Equals(other.Constant);
        
        public override int GetHashCode()
        {
            return 1715951373 + EqualityComparer<IPayload>.Default.GetHashCode(Constant);
        }

        public IEnumerable<EvalArg> GetArgs()
        {
            yield break;
        }

        public EvalStateMapper GetMapper() => (ctx, args) => new EvalComplete(Constant);

        public static bool operator ==(EConstant expression1, EConstant expression2)
        {
            return EqualityComparer<EConstant>.Default.Equals(expression1, expression2);
        }

        public static bool operator !=(EConstant expression1, EConstant expression2)
        {
            return !(expression1 == expression2);
        }
    }
}
