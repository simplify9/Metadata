using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content
{
    public interface IContentNode
    {
        
        bool TryEvaluate(ContentPath path, out IContentNode result);
        
        

        ComparisonResult CompareWith(IContentNode other);
    }
}
