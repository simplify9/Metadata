using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public class EOr : BinaryFilterBase
    {
        string Enclose(IEvalExpression exp)
            => exp is EAnd
                ? $"({exp})"
                : exp.ToString();
        
        public EOr(IEvalExpression left, IEvalExpression right) : base(left, right)
        {

        }
        
        public override string ToString() => $"{Enclose(Left)} || {Enclose(Right)}";

        protected override bool IsMatch(bool left, bool right) => left || right;
    }
}
