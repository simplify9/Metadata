using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Expressions
{
    public class ExpressionIssue
    {
        public string Message { get; }

        public ExpressionIssue(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
