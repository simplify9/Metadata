
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Serialization
{
    public class Tokenizer
    {
        private List<TokenDefinition> _tokenDefinitions;

        public Tokenizer()
        {
            _tokenDefinitions = new List<TokenDefinition>();

            _tokenDefinitions.Add(new TokenDefinition(TokenType.And, "and", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Or, "or", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Null, "null", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.OpenBracket, "\\(", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.CloseBracket, "\\)", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Identifier, "([a-z]|[A-Z]|_)+([a-z]|[A-Z]|_|[0-9])*", 3));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Path, @"(\.([a-z]|[A-Z]|_)+([a-z]|[A-Z]|_|[0-9])*)+", 2));

            //_tokenDefinitions.Add(new TokenDefinition(TokenType.Path, @"([a-z]|[A-Z]|_)+([a-z]|[A-Z]|_|[0-9])*(\.([a-z]|[A-Z]|_)+([a-z]|[A-Z]|_|[0-9])*)*", 2));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Contains, "contains", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Equals, "equals|=", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.DateTime, ContentDateTime.Regex, 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.String, "\"((\\.)|[^\\\\\"])*\"", 0));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Number, @"[\+\-]?\d+\.?[Ee]?[\+\-]?\d*", 2));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.TrueLiteral, "true", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.TrueLiteral, "false", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Comma, ",", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Colon, ":", 1));
            //_tokenDefinitions.Add(new TokenDefinition(TokenType.Dot, "\\.", 3));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.SpreadDots, "\\.{3}", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.OpenCurly, "\\{", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.CloseCurly, "\\}", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.OpenSquareBracket, "\\[", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.CloseSquareBracket, "\\]", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.DollarSign, "\\$", 1));


            //_tokenDefinitions.Add(new TokenDefinition(TokenType.Application, "app|application", 1));
            //_tokenDefinitions.Add(new TokenDefinition(TokenType.Between, "between", 1));
            //_tokenDefinitions.Add(new TokenDefinition(TokenType.CloseParenthesis, "\\)", 1));
           
            //_tokenDefinitions.Add(new TokenDefinition(TokenType.Equals, "=", 1));
            //_tokenDefinitions.Add(new TokenDefinition(TokenType.ExceptionType, "ex|exception", 1));
            //_tokenDefinitions.Add(new TokenDefinition(TokenType.Fingerprint, "fingerprint", 1));
            //_tokenDefinitions.Add(new TokenDefinition(TokenType.NotIn, "not in", 1));
            //_tokenDefinitions.Add(new TokenDefinition(TokenType.In, "in", 1));
            //_tokenDefinitions.Add(new TokenDefinition(TokenType.Like, "like", 1));
            //_tokenDefinitions.Add(new TokenDefinition(TokenType.Limit, "limit", 1));
            //_tokenDefinitions.Add(new TokenDefinition(TokenType.Match, "match", 1));
            //_tokenDefinitions.Add(new TokenDefinition(TokenType.Message, "msg|message", 1));
            //_tokenDefinitions.Add(new TokenDefinition(TokenType.NotEquals, "!=", 1));
            //_tokenDefinitions.Add(new TokenDefinition(TokenType.NotLike, "not like", 1));
            //_tokenDefinitions.Add(new TokenDefinition(TokenType.OpenParenthesis, "\\(", 1));
            //_tokenDefinitions.Add(new TokenDefinition(TokenType.Or, "or", 1));
            //_tokenDefinitions.Add(new TokenDefinition(TokenType.StackFrame, "sf|stackframe", 1));
            //_tokenDefinitions.Add(new TokenDefinition(TokenType.DateTimeValue, "\\d\\d\\d\\d-\\d\\d-\\d\\d \\d\\d:\\d\\d:\\d\\d", 2));
            //_tokenDefinitions.Add(new TokenDefinition(TokenType.StringValue, "'([^']*)'", 1));
            //_tokenDefinitions.Add(new TokenDefinition(TokenType.Number, "\\d+", 2));
        }

        public IEnumerable<DslToken> Tokenize(string lqlText)
        {
            var tokenMatches = FindTokenMatches(lqlText);

            var groupedByIndex = tokenMatches.GroupBy(x => x.StartIndex)
                .OrderBy(x => x.Key)
                .ToList();

            TokenMatch lastMatch = null;
            for (int i = 0; i < groupedByIndex.Count; i++)
            {
                var bestMatch = groupedByIndex[i].OrderBy(x => x.Precedence).First();
                if (lastMatch != null && bestMatch.StartIndex < lastMatch.EndIndex)
                    continue;

                yield return new DslToken(bestMatch);

                lastMatch = bestMatch;
            }

            //yield return new DslToken(TokenType.SequenceTerminator);
        }

        private TokenMatch[] FindTokenMatches(string text)
        {
            return _tokenDefinitions
                .SelectMany(def => def.FindMatches(text))
                .ToArray();
        }
    }
}
