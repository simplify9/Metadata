using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval.Parser.Internal
{
    public delegate IParserResult<T> Parser<out T>(string input);
}
