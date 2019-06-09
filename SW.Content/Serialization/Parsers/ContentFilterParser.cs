
using SW.Content.Filters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SW.Content.Serialization
{
    

    public class ContentFilterParser
    {
        
        readonly Tokenizer _tokenizer;
        
        public ContentFilterParser(Tokenizer tokenizer)
        {
            _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
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
            
            var value = constToken.CreateValue();
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

        public ParserIssue[] TryParse(string text, out IContentFilter result)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            result = null;

            var tokens = _tokenizer.Tokenize(text);

            try
            {
                var q = new Queue<DslToken>(tokens);

                result = ParseExpression(q);

                return new ParserIssue[] { };
            }
            catch (Exception ex)
            {
                return new ParserIssue[] { new ParserIssue { CodeOrMessage = ex.Message } };
            }
        }
        
        
    }

}
