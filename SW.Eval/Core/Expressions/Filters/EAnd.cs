using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SW.Eval
{
    public class EAnd : BinaryFilterBase
    {
        string Enclose(IEvalExpression exp)
            => exp is EOr
                ? $"({exp})"
                : exp.ToString();

        public EAnd(IEvalExpression left, IEvalExpression right) : base(left, right)
        {

        }
        
        public override string ToString() => $"{Enclose(Left)} && {Enclose(Right)}";

        protected override bool IsMatch(bool left, bool right) => left && right;
    }
}
