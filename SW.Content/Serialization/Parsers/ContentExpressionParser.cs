using SW.Content.Expressions;
using SW.Content.Filters;
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
            var keyToken = q.DequeueAndValidate(TokenType.Identifier);
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
                else if (lookAhead.TokenType == TokenType.Identifier)
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
                    throw new ParserException($"Unexpected token", lookAhead);
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
                    if (exp != null) throw new ParserException($"Unexpected token", lookAhead);
                    var token = q.DequeueAndValidate(Utils.ConstantTokens);
                    exp = CreateConstant(token);
                }

                else if (lookAhead.TokenType == TokenType.Equals)
                {
                    if (exp == null) throw new ParserException($"Unexpected token", lookAhead);
                    var token = q.DequeueAndValidate(TokenType.Equals);
                    exp = new EqualToFilter(exp, 
                        ParseExpression(q, t => t.TokenType == TokenType.And || t.TokenType == TokenType.Or || terminate(t)));
                }

                else if (lookAhead.TokenType == TokenType.Contains)
                {
                    if (exp == null) throw new ParserException($"Unexpected token", lookAhead);
                    var token = q.DequeueAndValidate(TokenType.Contains);
                    exp = new ContainsFilter(exp, 
                        ParseExpression(q, t => t.TokenType == TokenType.And || t.TokenType == TokenType.Or || terminate(t)));
                }

                else if (lookAhead.TokenType == TokenType.MatchRegex)
                {
                    if (exp == null) throw new ParserException($"Unexpected token", lookAhead);
                    var token = q.DequeueAndValidate(TokenType.MatchRegex);
                    var pattern = q.DequeueAndValidate(TokenType.String);
                    exp = new RegexFilter(exp, pattern.Value);
                }

                else if (lookAhead.TokenType == TokenType.And)
                {
                    if (exp == null) throw new ParserException($"Unexpected token", lookAhead);
                    var token = q.DequeueAndValidate(TokenType.And);
                    if (exp is IContentFilter leftBool)
                    {
                        var right = ParseExpression(q, terminate);
                        if (right is IContentFilter rightBool)
                        {
                            exp = new AndFilter(leftBool, rightBool);
                        }
                        else
                        {
                            throw new ParserException("AND cannot have a right operand that does not yield boolean", token);
                        }
                    }
                    else
                    {
                        throw new ParserException("AND cannot have a left operand that does not yield boolean", token);
                    }
                }

                else if (lookAhead.TokenType == TokenType.Or)
                {
                    if (exp == null) throw new ParserException($"Unexpected token", lookAhead);
                    var token = q.DequeueAndValidate(TokenType.Or);
                    if (exp is IContentFilter leftBool)
                    {
                        var right = ParseExpression(q, terminate);
                        if (right is IContentFilter rightBool)
                        {
                            exp = new OrFilter(leftBool, rightBool);
                        }
                        else
                        {
                            throw new ParserException("OR cannot have a right operand that does not yield boolean", token);
                        }
                    }
                    else
                    {
                        throw new ParserException("OR cannot have a left operand that does not yield boolean", token);
                    }
                }

                else if (lookAhead.TokenType == TokenType.Path)
                {
                    if (exp == null) throw new ParserException($"Unexpected token", lookAhead);
                    var token = q.DequeueAndValidate(TokenType.Path);
                    exp = new PathExpression(exp, ContentPath.Parse($"$.{token.Value.TrimStart('.')}"));
                }

                else if (lookAhead.TokenType == TokenType.DollarSign)
                {
                    if (exp != null) throw new ParserException($"Unexpected token", lookAhead);
                    q.DequeueAndValidate(TokenType.DollarSign);
                    exp = new ScopeRootExpression();
                }

                else if (lookAhead.TokenType == TokenType.OpenCurly)
                {
                    if (exp != null) throw new ParserException($"Unexpected token", lookAhead);
                    exp = CreateObjectExpression(q);
                }

                else if (lookAhead.TokenType == TokenType.OpenSquareBracket)
                {
                    if (exp != null) throw new ParserException($"Unexpected token", lookAhead);
                    exp = CreateListExpression(q);
                }
                
                else if (lookAhead.TokenType == TokenType.OpenBracket)
                {
                    if (exp != null) throw new ParserException($"Unexpected token", lookAhead);
                    q.DequeueAndValidate(TokenType.OpenBracket); // open bracket (
                    exp = ParseExpression(q, t => t.TokenType == TokenType.CloseBracket);
                    q.DequeueAndValidate(TokenType.CloseBracket); // close bracket )
                }
                else
                {
                    throw new ParserException($"Unexpected token", lookAhead);
                }
            }

            if (exp == null) throw new ParserException($"Unexpected end", null);

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
            catch (ParserException ex)
            {
                return new ParserIssue[] 
                {
                    new ParserIssue
                    {
                        CodeOrMessage = ex.Message,
                        Index = ex.Token?.Match.StartIndex ?? (text.Length - 1),
                        Value = ex.Token?.Value
                    }
                };
            }
        }
    }
}
