using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval.Core.Expressions.Parsing.Experimental
{
    public static class Parsers
    {
        public static Parser<T> Return<T>(T v) => i =>
        {
            
            return new ParserResult<T>(v, i, true);
        };

        public static Parser<T> Failure<T>() => i => new ParserResult<T>(default, i, false);
        
        public static Parser<char> Item { get; } =
            i => i.Length < 1
                ? Failure<char>()(i)
                : Return(i[0])(i.Substring(1));

        public static Parser<T> Or<T>(this Parser<T> p, Parser<T> defaultParser)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));
            if (defaultParser == null) throw new ArgumentNullException(nameof(defaultParser));
            
            return i =>
              {
                  var result = p(i);
                  return !result.IsSuccessful ? defaultParser(i) : result;
              };
        }
        

        public static Parser<T[]> OneOrMore<T>(this Parser<T> p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));
            
            return i =>
            {
                var state = Tuple.Create(new T[] { }, i);
                while (true)
                {
                    var newState = p(state.Item2);
                    if (!newState.IsSuccessful) break;
                    state = Tuple.Create(state.Item1.Concat(new[] { newState.Value })
                        .ToArray(), 
                            newState.Remaining);
                }

                if (state.Item1.Length < 1) return Failure<T[]>()(state.Item2);
                return Return(state.Item1)(state.Item2);
            };
        }

        public static Parser<T[]> ZeroOrMore<T>(this Parser<T> p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));
            return p.OneOrMore().Or(Return(Array.Empty<T>()));
        }

        public static Parser<T2> Bind<T1, T2>(this Parser<T1> p, Func<T1, Parser<T2>> f)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));
            if (f == null) throw new ArgumentNullException(nameof(f));
            
            return i =>
            {
                if (i == null) throw new ArgumentNullException(nameof(i));
                
                var result = p(i);
                return !result.IsSuccessful
                    ? Failure<T2>()(i)
                    : f(result.Value)(result.Remaining);
            };
        }

        public static Parser<TResult> Select<TSource, TResult>(this Parser<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            
            return i =>
            {

                var result = source(i);
                return !result.IsSuccessful
                    ? Failure<TResult>()(i)
                    : Return(selector(result.Value))(result.Remaining);
            };
        }

        public static Parser<TResult> SelectMany<TSource, TValue, TResult>(this Parser<TSource> source, Func<TSource, Parser<TValue>> valueSelector, Func<TSource, TValue, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (valueSelector == null) throw new ArgumentNullException(nameof(valueSelector));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            
            return source.Bind(s => valueSelector(s).Select(v => resultSelector(s, v)));
        }

        public static Parser<char> Char(Predicate<char> p) 
            => Item.Bind(c => p(c) 
                ? Return(c) 
                : Failure<char>());

        public static Parser<char> Char(char c) => Char(i => i == c);
        
        public static Parser<char> Whitespace { get; } = Char(c => char.IsWhiteSpace(c) || c == '\0').ZeroOrMore().Select(_ => ' ');

        public static Parser<T> Token<T>(this Parser<T> p)
            =>  from _ in Whitespace
                from t in p
                from __ in Whitespace
                from result in Return(t)
                select result;
        
        public static Parser<string> String(string str)
            => i => !i.StartsWith(str)
                ? Failure<string>()(i)
                : Return(str)(i.Substring(str.Length));


        public static Parser<IEnumerable<T>> DelimitedBy<T, U>(this Parser<T> parser, Parser<U> delimiter)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (delimiter == null) throw new ArgumentNullException(nameof(delimiter));

            return (from head in parser.Select(t => new T[] { t }) 
                   from tail in
                       (from separator in delimiter
                        from item in parser
                        select item).ZeroOrMore()
                   select head.Concat(tail)).Or(Return(Array.Empty<T>()));
        }



        public static Parser<string> Symbol(string xs) => String(xs).Token();

        // basic types

        

        

        public static Parser<char> Digit { get; } = Char(char.IsDigit);

        public static Parser<char> Lower { get; } = Char(char.IsLower);

        public static Parser<char> Upper { get; } = Char(char.IsUpper);

        public static Parser<char> Letter { get; } = Char(char.IsLetter);

        public static Parser<char> AlphaNum { get; } = Char(char.IsLetterOrDigit);

        
    }
}
