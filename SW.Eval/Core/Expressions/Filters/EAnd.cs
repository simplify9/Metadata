using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SW.Eval
{
    public class EAnd : BinaryFilterBase
    {
        string Enclose(IEvalFilter exp)
            => exp is EOr
                ? $"({exp})"
                : exp.ToString();

        public EAnd(IEvalFilter left, IEvalFilter right) : base(left, right)
        {

        }
        
        public override string ToString() => $"{Enclose(Left)} AND {Enclose(Right)}";

        protected override bool IsMatch(bool left, bool right) => left && right;
    }
}
