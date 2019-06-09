using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SW.Content.Serialization
{
    static class Utils
    {
        public static readonly TokenType[] ConstantTokens = new[]
        {
            TokenType.DateTime,
            TokenType.String,
            TokenType.Number,
            TokenType.Null,
            TokenType.TrueLiteral,
            TokenType.FalseLiteral
        };

        public static bool IsConstant(this DslToken token)
        {
            return ConstantTokens.Contains(token.TokenType);
        }

        public static IContentNode CreateValue(this DslToken token)
        {
            switch (token.TokenType)
            {
                case TokenType.DateTime: return new ContentDateTime(DateTime.Parse(token.Value, null, DateTimeStyles.RoundtripKind));
                case TokenType.String: return new ContentText(token.Value.Substring(1, token.Value.Length - 2));
                case TokenType.Number: return new ContentNumber(decimal.Parse(token.Value));
                case TokenType.Null: return new ContentNull();
                case TokenType.TrueLiteral: return new ContentBoolean(true);
                case TokenType.FalseLiteral: return new ContentBoolean(false);
                default: throw new ArgumentException($"Unexpected token");
            }
        }

        public static DslToken DequeueAndValidate(this Queue<DslToken> q, params TokenType[] expectedTypes)
        {
            if (q.Count < 1)
            {
                throw new ArgumentException("Unexpected end");
            }

            var next = q.Peek();
            if (!expectedTypes.Contains(next.TokenType))
            {
                throw new ArgumentException($"Unexpected {next.TokenType}");
            }

            return q.Dequeue();
        }
    }
}
