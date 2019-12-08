using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public class AndFilter : BinaryFilterBase
    {
        string Enclose(IEvalFilter exp)
            => exp is OrFilter
                ? $"({exp})"
                : exp.ToString();

        public AndFilter(IEvalFilter left, IEvalFilter right) : base(left, right)
        {

        }
        
        public override string ToString() => $"{Enclose(Left)} AND {Enclose(Right)}";

        protected override bool IsMatch(bool left, bool right) => left && right;
    }
}
