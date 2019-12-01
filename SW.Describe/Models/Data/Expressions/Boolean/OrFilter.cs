using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    public class OrFilter : BinaryFilterBase
    {
        string Enclose(IContentFilter exp)
        {
            if (exp is AndFilter) return $"({exp})";
            return exp.ToString();
        }

        public OrFilter(IContentFilter left, IContentFilter right) 
            : base(ContentFilterType.Or, left, right)
        {

        }
        
        public override bool IsMatch(IPayload document)
        {
            return Left.IsMatch(document) || Right.IsMatch(document);
        }

        public override string ToString()
        {
            return $"{Enclose(Left)} OR {Enclose(Right)}";
        }
    }
}
