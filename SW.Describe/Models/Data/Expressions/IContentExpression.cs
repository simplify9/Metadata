
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    public interface IContentExpression
    {
        

        ExpressionIssue TryEvaluate(IPayload node, out IPayload result);
    }
}
