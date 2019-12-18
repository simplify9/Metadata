using SW.Eval.Parser.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval.Parser.Grammar
{
    public static class Tokens
    {

        static Parser<char> DoubleQuote => Parsers.Char('"');
        static Parser<char> Underscore => Parsers.Char('_');
        

        public static Parser<char> BracketOpen => Parsers.Char('(').Token();

        public static Parser<char> BracketClose => Parsers.Char(')').Token();

        public static Parser<char> SquareBracketOpen => Parsers.Char('[').Token();

        public static Parser<char> SquareBracketClose => Parsers.Char(']').Token();

        public static Parser<char> CurlyOpen => Parsers.Char('{').Token();

        public static Parser<char> CurlyClose => Parsers.Char('}').Token();

        public static Parser<char> Comma => Parsers.Char(',').Token();

        public static Parser<char> Colon => Parsers.Char(':').Token();

        public static Parser<char> Dot => Parsers.Char('.').Token();

        public static Parser<string> DotSpread => Parsers.Symbol("...");

        public static Parser<string> NullLiteral => Parsers.Symbol("null");

        public static Parser<string> Lambda => Parsers.Symbol("=>");

        public static Parser<string> LogicalAnd => Parsers.Symbol("&&");

        public static Parser<string> LogicalOr => Parsers.Symbol("||");

        public static Parser<bool> BooleanLiteral =>
            from symbol in Parsers.Symbol("true").Or(Parsers.Symbol("false"))
            select symbol == "true";

        public static Parser<string> Identifier =>
            (from first in Parsers.Letter.Or(Underscore).Or(Parsers.Char('$'))
             from restOf in Parsers.AlphaNum.Or(Underscore).ZeroOrMore()
             from result in Parsers.Return($"{first}{new string(restOf)}")
             select result).Token();
        
        public static Parser<string> StringLiteral =>
            (from _ in DoubleQuote
             from chars in Parsers.Char(c => c != '"' && c != '\\')
                 .Or(Parsers.String("\\'").Select(s => '\''))
                 .Or(Parsers.String("\\\\").Select(s => '\\'))
                 .Or(Parsers.String("\\\"").Select(s => '"'))
                 .Or(Parsers.String("\\'").Select(s => '\''))
                 .Or(Parsers.String("\\0").Select(s => '\0'))
                 .Or(Parsers.String("\\a").Select(s => '\a'))
                 .Or(Parsers.String("\\b").Select(s => '\b'))
                 .Or(Parsers.String("\\f").Select(s => '\f'))
                 .Or(Parsers.String("\\n").Select(s => '\n'))
                 .Or(Parsers.String("\\r").Select(s => '\r'))
                 .Or(Parsers.String("\\t").Select(s => '\t'))
                 .Or(Parsers.String("\\v").Select(s => '\v'))
                 .ZeroOrMore()
             from __ in DoubleQuote
             select new string(chars)).Token();

        public static Parser<decimal> NumberLiteral =>
            (from sign in Parsers.Char('-').Select(_ => "-").Or(Parsers.Return(string.Empty))
             from digits in Parsers.Digit.OneOrMore().Select(chars => new string(chars))
             from _ in Parsers.Char('.').Or(Parsers.Return('.'))
             from fractions in Parsers.Digit.ZeroOrMore().Select(chars => new string(chars))
             select decimal.Parse($"{sign}{digits}.{fractions}0")).Token();
    }
}
