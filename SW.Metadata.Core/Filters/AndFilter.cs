using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core
{
    public class AndFilter : BinaryFilterBase
    {
        string Enclose(IDocumentFilter exp)
        {
            if (exp is OrFilter) return $"({exp})";
            return exp.ToString();
        }

        public AndFilter(IDocumentFilter left, IDocumentFilter right) 
            : base(DocumentFilterType.And, left, right)
        {

        }
        
        public override bool IsMatch(DocumentContentReader document)
        {
            return Left.IsMatch(document) && Right.IsMatch(document);
        }

        public override string ToString()
        {
            return $"{Enclose(Left)} AND {Enclose(Right)}";
        }
        
    }
}
