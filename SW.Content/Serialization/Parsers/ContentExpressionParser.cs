using SW.Content.Expressions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SW.Content.Serialization
{
    public class ContentExpressionParser
    {
        readonly Tokenizer _tokenizer;

        ConstantExpression CreateConstant(DslToken token)
        {
            return new ConstantExpression(token.CreateValue());
        }

        PathExpression CreatePathExpression(DslToken token)
        {
            return new PathExpression(ContentPath.Parse(token.Value), new ScopeRootExpression());
        }

        CreateListExpression CreateListExpression(Queue<DslToken> q)
        {
            var items = new List<CreateListExpression.Element>();
            
            q.DequeueAndValidate(TokenType.OpenSquareBracket);

            while (true)
            {
                var lookAhead = q.Peek();

                if (lookAhead.TokenType == TokenType.Comma)
                {
                    q.DequeueAndValidate(TokenType.Comma);
                }
                else if (lookAhead.TokenType == TokenType.CloseSquareBracket)
                {
                    break;
                }
                else if (lookAhead.TokenType == TokenType.SpreadDots)
                {
                    q.DequeueAndValidate(TokenType.SpreadDots);
                    var exp = ParseExpression(q, t =>
                        t.TokenType == TokenType.CloseSquareBracket ||
                        t.TokenType == TokenType.Comma);
                    items.Add(new CreateListExpression.List { Value = exp });
                }
                else
                {
                    var exp = ParseExpression(q, t =>
                        t.TokenType == TokenType.CloseSquareBracket ||
                        t.TokenType == TokenType.Comma);
                    items.Add(new CreateListExpression.ListItem { Value = exp });
                }
            }

            q.DequeueAndValidate(TokenType.CloseSquareBracket);

            return new CreateListExpression(items);
        }

        KeyValuePair<string,IContentExpression> CreateAttribute(Queue<DslToken> q)
        {
            var keyToken = q.DequeueAndValidate(TokenType.Path);
            var key = keyToken.Value;

            q.DequeueAndValidate(TokenType.Colon);

            var exp = ParseExpression(q, t => 
                t.TokenType == TokenType.CloseCurly || 
                t.TokenType == TokenType.Comma);

            return new KeyValuePair<string, IContentExpression>(key, exp);
        }

        CreateObjectExpression CreateObjectExpression(Queue<DslToken> q)
        {
            var attributes = new List<CreateObjectExpression.Element>();

            q.DequeueAndValidate(TokenType.OpenCurly);
            
            while (true)
            {
                var lookAhead = q.Peek();

                if (lookAhead.TokenType == TokenType.Comma)
                {
                    q.DequeueAndValidate(TokenType.Comma);
                }
                else if (lookAhead.TokenType == TokenType.SpreadDots)
                {
                    q.DequeueAndValidate(TokenType.SpreadDots);
                    attributes.Add(new CreateObjectExpression.Object
                    {
                        SubObject = ParseExpression(q, t =>
                            t.TokenType == TokenType.CloseCurly ||
                            t.TokenType == TokenType.Comma)
                    });
                }
                else if (lookAhead.TokenType == TokenType.Path)
                {
                    var a = CreateAttribute(q);
                    attributes.Add(new CreateObjectExpression.Attribute
                    {
                        Name = a.Key,
                        Value = a.Value
                    });
                }
                else if (lookAhead.TokenType == TokenType.CloseCurly)
                {
                    break;
                }
                else
                {
                    throw new ArgumentException($"Unexpected {lookAhead.TokenType}");
                }
            }

            q.DequeueAndValidate(TokenType.CloseCurly);

            return new CreateObjectExpression(attributes);
        }
        

        public ContentExpressionParser(Tokenizer tokenizer)
        {
            _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
        }

        IContentExpression ParseExpression(Queue<DslToken> q, Func<DslToken,bool> terminate)
        {
            IContentExpression exp = null;
            
            while (q.Count > 0)
            {
                var lookAhead = q.Peek();

                if (terminate(lookAhead)) break;

                else if (lookAhead.IsConstant())
                {
                    var token = q.DequeueAndValidate(Utils.ConstantTokens);
                    exp = CreateConstant(token);
                }

                else if (lookAhead.TokenType == TokenType.Path)
                {
                    var token = q.DequeueAndValidate(TokenType.Path);
                    exp = CreatePathExpression(token);
                }

                else if (lookAhead.TokenType == TokenType.OpenCurly)
                {
                    exp = CreateObjectExpression(q);
                }

                else if (lookAhead.TokenType == TokenType.OpenSquareBracket)
                {
                    exp = CreateListExpression(q);
                }
                
                else if (lookAhead.TokenType == TokenType.OpenBracket)
                {
                    q.DequeueAndValidate(TokenType.OpenBracket); // open bracket (
                    exp = ParseExpression(q, t => t.TokenType == TokenType.CloseBracket);
                    q.DequeueAndValidate(TokenType.CloseBracket); // close bracket )
                }
                else
                {
                    throw new ArgumentException($"Unexpected {lookAhead.TokenType}");
                }
            }

            if (exp == null) throw new ArgumentException("Unexpected end of text");

            return exp;
        }

        public ParserIssue[] TryParse(string text, out IContentExpression result)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            result = null;

            var tokens = _tokenizer.Tokenize(text);

            try
            {
                var q = new Queue<DslToken>(tokens);

                result = ParseExpression(q, t => false);

                return new ParserIssue[] { };
            }
            catch (Exception ex)
            {
                return new ParserIssue[] { new ParserIssue { CodeOrMessage = ex.Message } };
            }
        }
    }
}
