using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core
{
    public interface IDocumentValue
    {
        IEnumerable<IDocumentValue> AsEnumerable(Func<object,IDocumentValue> wrapIn);

        bool TryEvaluate(DocumentPath path, Func<object, IDocumentValue> wrapIn, out IDocumentValue result);
        
        string CreateMatchKey();

        ComparisonResult CompareWith(IDocumentValue other);
    }
}
