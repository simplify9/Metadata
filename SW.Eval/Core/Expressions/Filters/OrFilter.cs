using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public class OrFilter : BinaryFilterBase
    {
        string Enclose(IEvalFilter exp)
            => exp is AndFilter
                ? $"({exp})"
                : exp.ToString();
        
        public OrFilter(IEvalFilter left, IEvalFilter right) : base(left, right)
        {

        }
        
        public override string ToString() => $"{Enclose(Left)} OR {Enclose(Right)}";

        protected override bool IsMatch(bool left, bool right) => left || right;
    }
}
