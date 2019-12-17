using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval.Expressions.Parsing
{
    public class EvalExpressionParser
    {
        
        
        public static ParserResult<IEvalExpression> Parse(IEvalExpression lhs, IEnumerable<DslToken> source)
        {
            return source.ParseExpressionTree();
        }
    }
}
