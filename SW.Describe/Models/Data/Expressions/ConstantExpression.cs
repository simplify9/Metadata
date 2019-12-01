using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    public class ConstantExpression : IContentExpression, IEquatable<ConstantExpression>
    {
        public IPayload Constant { get; }

        public ConstantExpression(IPayload constant)
        {
            Constant = constant ?? throw new ArgumentNullException(nameof(constant));
        }

        public ExpressionIssue TryEvaluate(IPayload input, out IPayload result)
        {
            result = Constant;

            return null;
        }

        public override string ToString()
        {
            return Constant.ToString();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ConstantExpression);
        }

        public bool Equals(ConstantExpression other)
        {
            return other != null && Constant.Equals(other.Constant);
        }

        public override int GetHashCode()
        {
            return 1715951373 + EqualityComparer<IPayload>.Default.GetHashCode(Constant);
        }
        
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
