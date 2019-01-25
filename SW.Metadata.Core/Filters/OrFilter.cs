using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core
{
    public class OrFilter : BinaryFilterBase
    {
        string Enclose(IDocumentFilter exp)
        {
            if (exp is AndFilter) return $"({exp})";
            return exp.ToString();
        }

        public OrFilter(IDocumentFilter left, IDocumentFilter right) 
            : base(DocumentFilterType.Or, left, right)
        {

        }
        
        public override bool IsMatch(DocumentContentReader document)
        {
            return Left.IsMatch(document) || Right.IsMatch(document);
        }

        public override string ToString()
        {
            return $"{Enclose(Left)} OR {Enclose(Right)}";
        }
    }
}
