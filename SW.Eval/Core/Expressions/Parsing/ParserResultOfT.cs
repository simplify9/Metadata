using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval.Expressions.Parsing
{
    public static class ParserResult
    {
        public static ParserResult<T> None<T>(IEnumerable<DslToken> remaining)
        {
            return new ParserResult<T>(remaining, default, default);
        }

        public static ParserResult<T> Success<T>(IEnumerable<DslToken> remaining, T result)
        {
            return new ParserResult<T>(remaining, result, default);
        }

        public static ParserResult<T> HasError<T>(IEnumerable<DslToken> remaining, string message)
        {
            return new ParserResult<T>(remaining, default, message);
        }
    }

    public class ParserResult<T>
    {
        public ParserResult(IEnumerable<DslToken> remaining, T result, string error)
        {
            RemainingTokens = remaining;
            Value = result;
            Error = error;
        }
        
        public IEnumerable<DslToken> RemainingTokens { get; }

        public T Value { get; }

        public string Error { get; }

        public ParserResult<K> Then<K>(Func<T, IEnumerable<DslToken>, ParserResult<K>> next)
        {
            if (Error != null) ParserResult.HasError<K>(RemainingTokens, Error);
            if (RemainingTokens.Any()) return next(Value, RemainingTokens);
            return ParserResult.HasError<K>(RemainingTokens, "Unexpected end of text");
        }

        public ParserResult<K> Then<K>(Func<T, K> next)
        {
            return Then((left, rem) => ParserResult.Success(rem, next(left)));
        }

        public ParserResult<T> ParseIf(Func<DslToken,bool> condition, 
            Func<ParserResult<T>, ParserResult<T>> next,
            Func<ParserResult<T>, ParserResult<T>> orElse = null)
        {
            if (Value != null && Error != null) return this;
            if (!RemainingTokens.Any()) return ParserResult.HasError<T>(RemainingTokens, "Unexpected end of text");
            return condition(RemainingTokens.First())
                ? next(this)
                : (orElse == null? this : orElse(this));
        }

        public ParserResult<K> Expect<K>(Func<DslToken, bool> condition, Func<DslToken,K> tokenParser)
        {
            return Then((lhs, remaining) =>
            {
                var next = RemainingTokens.First();
                return condition(next)
                    ? ParserResult.HasError<K>(RemainingTokens, $"Unexpected {next.TokenType}")
                    : ParserResult.Success(RemainingTokens.Skip(1), tokenParser(next));
            });
                
        }

    }
}
