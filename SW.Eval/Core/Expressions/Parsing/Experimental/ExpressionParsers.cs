using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval.Core.Expressions.Parsing.Experimental
{
    public static class ExpressionParsers
    {
        delegate IEvalExpression IteratorExpressionFactory(IEvalExpression left, ExpressionClosure closure);


        static Parser<char> DoubleQuote => Parsers.Char('"');
        static Parser<char> Underscore => Parsers.Char('_');

        // tokens

        static Parser<char> BracketOpen => Parsers.Char('(').Token();

        static Parser<char> BracketClose => Parsers.Char(')').Token();

        static Parser<char> SquareBracketOpen => Parsers.Char('[').Token();

        static Parser<char> SquareBracketClose => Parsers.Char(']').Token();

        static Parser<char> CurlyOpen => Parsers.Char('{').Token();

        static Parser<char> CurlyClose => Parsers.Char('}').Token();

        static Parser<char> Comma => Parsers.Char(',').Token();

        static Parser<char> Colon => Parsers.Char(':').Token();

        static Parser<char> Dot => Parsers.Char('.').Token();
        
        static Parser<string> DotSpread => Parsers.Symbol("...");

        static Parser<string> NullLiteral => Parsers.Symbol("null");

        static Parser<string> Lambda => Parsers.Symbol("=>");

        static Parser<string> LogicalAnd => Parsers.Symbol("&&");

        static Parser<string> LogicalOr => Parsers.Symbol("||");

        static Parser<bool> BooleanLiteral =>
            from symbol in Parsers.Symbol("true").Or(Parsers.Symbol("false"))
            select symbol == "true";
        
        static Parser<string> Identifier =>
            (from first in Parsers.Letter.Or(Underscore).Or(Parsers.Char('$'))
            from restOf in Parsers.AlphaNum.Or(Underscore).ZeroOrMore()
            from result in Parsers.Return($"{first}{new string(restOf)}")
            select result).Token();

        //static Parser<PayloadPath> Member =>
        //    from _ in Dot
        //    from  in  (from idn in Identifier
        //    select PayloadPath.Parse($"$.{idn}");
             
        static Parser<string> StringLiteral =>
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

        static Parser<decimal> NumberLiteral =>
            (from sign in Parsers.Char('-').Select(_ => "-").Or(Parsers.Return(string.Empty))
             from digits in Parsers.Digit.OneOrMore().Select(chars => new string(chars))
             from _ in Parsers.Char('.').Or(Parsers.Return('.'))
             from fractions in Parsers.Digit.ZeroOrMore().Select(chars => new string(chars))
             select decimal.Parse($"{sign}{digits}.{fractions}0")).Token();
                

        // expressions

        static Parser<EConstant> Constant =>
            from constant in NullLiteral.Select(v => new EConstant(PayloadNull.Singleton))
                .Or(BooleanLiteral.Select(v => EConstant.From(v)))
                .Or(NumberLiteral.Select(v => EConstant.From(v)))
                .Or(StringLiteral.Select(v => EConstant.From(v)))
            select constant;

        static Parser<EObject.Element> ObjectAttribute =>
            from attrName in Identifier.Or(StringLiteral)
            from _ in Colon
            from expr in Expression
            select new EObject.Attribute { Name = attrName, Value = expr };
        
        static Parser<EObject.Element> ObjectFragment =>
            from _ in DotSpread
            from expr in Expression
            select new EObject.Object { Value = expr };

        static Parser<EObject.Element> ObjectElement =>
            from element in ObjectAttribute.Or(ObjectFragment)
            select element;

        static Parser<EObject> Object =>
            from _ in CurlyOpen
            from elements in ObjectElement.DelimitedBy(Comma)
            from __ in CurlyClose
            select new EObject(elements);

        // arrays

        static Parser<EArray.Element> ArrayItem =>
            from expr in Expression
            select new EArray.ListItem { Value = expr };

        static Parser<EArray.Element> ArrayFragment =>
            from _ in DotSpread
            from expr in Expression
            select new EArray.List { Value = expr };

        static Parser<EArray.Element> ArrayElement =>
            from element in ArrayItem.Or(ArrayFragment)
            select element;

        static Parser<EArray> Array_ =>
            from _ in SquareBracketOpen
            from elements in ArrayElement.DelimitedBy(Comma)
            from __ in SquareBracketClose
            select new EArray(elements);

        // functions

        static Parser<string[]> ClosureWithBrackets =>
            from _ in BracketOpen
            from parameters in Identifier.DelimitedBy(Comma)
            from __ in BracketClose
            select parameters.ToArray();
        
        static Parser<ExpressionClosure> Closure =>
            from parameters in Identifier.Select(i => new [] { i }).Or(ClosureWithBrackets)
            from _ in Lambda
            from expr in Expression
            select new ExpressionClosure(expr, parameters);

        static Parser<EvalArg> FunctionCallArg =>
            from argName in Identifier
            from _ in Colon
            from argValue in Expression
            select new EvalArg(argName, argValue);

        static Parser<IEvalExpression> FunctionCall =>
            from funcName in Identifier
            from _ in BracketOpen
            from args in FunctionCallArg.DelimitedBy(Comma)
            from __ in BracketClose
            select new ECall(funcName, args);

        static IteratorExpressionFactory GetExpressionFactory(string funcName)
        {
            switch (funcName)
            {
                case "map": return (l, r) => new EMap(l, r);
                case "filter": return (l, r) => new EFilter(l, r);
                case "any": return (l, r) => new EContainsWhere(l, r);
                default: throw new NotSupportedException($"Array iterator with name '{funcName}' is not supported");
            }
        }
        
        delegate IEvalExpression OpFactory(IEvalExpression left);
        
        static Parser<OpFactory> AndExpression =>
            from _ in LogicalAnd
            from right in Expression
            from result in Parsers.Return<OpFactory>(left => new EAnd(left, right))
            select result;

        static Parser<OpFactory> OrExpression =>
            from _ in LogicalOr
            from right in Expression
            from result in Parsers.Return<OpFactory>(left => new EOr(left, right))
            select result;

        static Parser<OpFactory> ArrayIterator =>
            from _ in Dot
            from idn in Identifier
            from __ in BracketOpen
            from c in Closure
            from ___ in BracketClose
            from result in Parsers.Return<OpFactory>(left => GetExpressionFactory(idn)(left, c))
            select result;

        static Parser<OpFactory> PropertyAccess =>
            from _ in Dot
            from idn in Identifier
            from result in Parsers.Return<OpFactory>(left => new EPath(left, PayloadPath.Parse($"$.{idn}")))
            select result;

        // E

        static Parser<IEvalExpression> Expression =>

            from left in

                Constant

                .Or(from _ in BracketOpen from e in Expression from __ in BracketClose select e)

                .Or(from fc in FunctionCall select fc)

                .Or(from idn in Identifier
                    from result in Parsers.Return(new EVar(idn))
                    select result)

                .Or(Object)

                .Or(Array_)

            from right in

                 OrExpression

                .Or(AndExpression)

                .Or(ArrayIterator)

                .Or(PropertyAccess)

                .ZeroOrMore()

            from result in Parsers.Return(right.Aggregate(left, (expr, fn) => fn(expr)))

            select result;

            
        public static IParserResult<ExpressionClosure> ParseFunc(string i) => Closure(i);
        
        public static IParserResult<IEvalExpression> ParseExpression(string i) => Expression(i);
        
    }
}
