using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Serialization
{
    public class ParserException : Exception
    {
        public DslToken Token { get; }

        public ParserException(string message, DslToken token)
            : base($"{message}{(token == null? string.Empty : $"(at character {token.Match.StartIndex}" )})")
        {
            Token = token;
        }
    }
}
