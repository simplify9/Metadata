using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval.Expressions.Parsing
{
    public enum TokenType
    {
        
        And,
        Or,
        Equals,
        Contains,
        Number,
        DateTime,
        String,
        Null,
        Path,
        OpenBracket,
        CloseBracket,
        TrueLiteral,
        FalseLiteral,
        
        SequenceTerminator,

        Comma,
        Colon,
        Dot,
        SpreadDots,
        OpenCurly,
        CloseCurly,
        OpenSquareBracket,
        CloseSquareBracket,
        DollarSign,

        MatchRegex,
        Identifier,
        Lambda
    }
}
