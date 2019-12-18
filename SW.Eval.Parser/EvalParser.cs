using SW.Eval.Parser.Grammar;
using SW.Eval.Parser.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval.Parser
{
    public class EvalParser
    {
        public IParserResult<DataFunc> ParseFunc(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            return Language.DataFuncP(text);
        }

        public IParserResult<IEvalExpression> ParseExpression(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            return Language.Expression(text);
        }
    }
}
