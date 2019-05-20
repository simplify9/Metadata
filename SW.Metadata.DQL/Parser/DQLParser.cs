using SW.Metadata.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SW.Metadata.DQL.Parser
{
    static class DslTokenQueueTExtensions
    {
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

    public class DQLParser
    {
        static readonly TokenType[] _constantNodes =
        {
            TokenType.String,
            TokenType.Number,
            TokenType.Null,
            TokenType.DateTime,
            TokenType.TrueLiteral,
            TokenType.FalseLiteral
        };
        
        public class Issue
        {
            public int Index { get; set; }

            public string Value { get; set; }

            public string CodeOrMessage { get; set; }
        }

        readonly Tokenizer _tokenizer;

        IContentNode CreateValue(DslToken token)
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

        public DQLParser(Tokenizer tokenizer)
        {
            _tokenizer = tokenizer;
        }

        private IContentFilter ParseLeafExpression(Queue<DslToken> q)
        {
            var pathToken = q.DequeueAndValidate(TokenType.Path);
            var opToken = q.DequeueAndValidate(TokenType.Contains, TokenType.Equals);
            var constToken = q.DequeueAndValidate(
                TokenType.OpenBracket,
                TokenType.DateTime, 
                TokenType.Number,
                TokenType.TrueLiteral,
                TokenType.FalseLiteral,
                TokenType.String,
                TokenType.Null);

            var path = ContentPath.Parse(pathToken.Value);

            if (constToken.TokenType == TokenType.OpenBracket)
            {
                if (opToken.TokenType != TokenType.Contains)
                {
                    throw new ArgumentException($"Unexpected {constToken.TokenType}");
                }

                var subExp = ParseExpression(q);
                q.DequeueAndValidate(TokenType.CloseBracket);
                return new ContainsWhereFilter(path, subExp);
            }
            
            var value = CreateValue(constToken);
            return opToken.TokenType == TokenType.Contains
                ? (IContentFilter)new ContainsFilter(path, value)
                : new EqualToFilter(path, value);
        }

        private IContentFilter ParseExpression(Queue<DslToken> q)
        {

            IContentFilter exp = null;
            while (q.Count > 0)
            {
                var lookAhead = q.Peek();
                
                if (lookAhead.TokenType == TokenType.Path) // Name = "John"
                {
                    exp = ParseLeafExpression(q);
                }
                else if (lookAhead.TokenType == TokenType.OpenBracket)
                {
                    q.DequeueAndValidate(TokenType.OpenBracket); // open bracket (
                    exp = ParseExpression(q);
                    q.DequeueAndValidate(TokenType.CloseBracket); // close bracket )
                }
                else if (lookAhead.TokenType == TokenType.And && exp != null)
                {
                    q.DequeueAndValidate(TokenType.And); // AND
                    var right = ParseExpression(q);
                    exp = new AndFilter(exp, right);
                }
                else if (lookAhead.TokenType == TokenType.Or && exp != null)
                {
                    q.DequeueAndValidate(TokenType.Or); // OR
                    var right = ParseExpression(q);
                    exp = new OrFilter(exp, right);
                }
                else if (lookAhead.TokenType == TokenType.CloseBracket)
                {
                    break;
                }
                else
                {
                    throw new ArgumentException($"Unexpected {lookAhead.TokenType}");
                }
            }

            if (exp == null) throw new ArgumentException("Unexpected end of text");

            return exp;
        }

        public Issue[] TryParse(string text, out IContentFilter result)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            result = null;

            var tokens = _tokenizer.Tokenize(text);

            try
            {
                var q = new Queue<DslToken>(tokens);

                result = ParseExpression(q);

                return new Issue[] { };
            }
            catch (Exception ex)
            {
                return new Issue[] { new Issue { CodeOrMessage = ex.Message } };
            }
        }
        
        
    }

}
