using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval.Core.Expressions.Parsing.Experimental
{
    public interface IParserResult<out T>
    {
        T Value { get; }

        string Remaining { get; }
        
        bool IsSuccessful { get; }
    }
}
