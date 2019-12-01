using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    public class AndFilter : BinaryFilterBase
    {
        string Enclose(IContentFilter exp)
        {
            if (exp is OrFilter) return $"({exp})";
            return exp.ToString();
        }

        public AndFilter(IContentFilter left, IContentFilter right) 
            : base(ContentFilterType.And, left, right)
        {

        }
        
        public override bool IsMatch(IPayload document)
        {
            return Left.IsMatch(document) && Right.IsMatch(document);
        }

        public override string ToString()
        {
            return $"{Enclose(Left)} AND {Enclose(Right)}";
        }
        
    }
}
