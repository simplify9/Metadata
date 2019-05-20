using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Filters.Parser
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
        SequenceTerminator
    }
}
