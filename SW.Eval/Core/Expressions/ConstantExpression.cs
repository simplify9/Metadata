using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public class ConstantExpression : IEvalExpression, IEquatable<ConstantExpression>
    {
        public IPayload Constant { get; }

        public ConstantExpression(IPayload constant)
        {
            Constant = constant ?? throw new ArgumentNullException(nameof(constant));
        }
        

        public override string ToString() => Constant.ToString();
        
        public override bool Equals(object obj) => Equals(obj as ConstantExpression);
        
        public bool Equals(ConstantExpression other) => other != null && Constant.Equals(other.Constant);
        
        public override int GetHashCode()
        {
            return 1715951373 + EqualityComparer<IPayload>.Default.GetHashCode(Constant);
        }

        public IEnumerable<IEvalExpression> GetChildren()
        {
            yield break;
        }

        public IEvalState ComputeState(EvalContext ctx) => new EvalComplete(Constant);

        public static bool operator ==(ConstantExpression expression1, ConstantExpression expression2)
        {
            return EqualityComparer<ConstantExpression>.Default.Equals(expression1, expression2);
        }

        public static bool operator !=(ConstantExpression expression1, ConstantExpression expression2)
        {
            return !(expression1 == expression2);
        }
    }
}
