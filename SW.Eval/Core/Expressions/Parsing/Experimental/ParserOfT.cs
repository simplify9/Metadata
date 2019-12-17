using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval.Core.Expressions.Parsing.Experimental
{
    public delegate IParserResult<T> Parser<out T>(string input);
}
