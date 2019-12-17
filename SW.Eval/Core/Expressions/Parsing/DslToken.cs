using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval.Expressions.Parsing
{
    public class DslToken
    {


        public DslToken(TokenType tokenType)
        {
            TokenType = tokenType;
            Value = string.Empty;
        }

        public DslToken(TokenMatch match)
        {
            TokenType = match.TokenType;
            Value = match.Value;
            Match = match;
        }

        public TokenMatch Match { get; }
        public TokenType TokenType { get;  }
        public string Value { get; }

        public DslToken Clone()
        {
            return new DslToken(Match);
        }

        public override string ToString()
        {
            return $"Type: {TokenType}, Value={Value}";
        }
    }
}
