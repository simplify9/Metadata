using SW.Eval.Binding;
using SW.Eval.Parser.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval.Parser.Grammar
{
    public static class Language
    {
        public delegate IEvalExpression IteratorFactory(IEvalExpression left, DataFunc closure);

        public delegate IEvalExpression OpFactory(IEvalExpression left);

        static IteratorFactory GetExpressionFactory(string funcName)
        {
            switch (funcName)
            {
                case "map": return (l, r) => new EMap(l, r);
                case "filter": return (l, r) => new EFilter(l, r);
                case "any": return (l, r) => new EAny(l, r);
                default: return (l, r) => new ENotImplemented(funcName);
            }
        }

        // expressions

        public static Parser<EConstant> Constant =>
            from constant in Tokens.NullLiteral.Select(v => new EConstant(PayloadNull.Singleton))
                .Or(Tokens.BooleanLiteral.Select(v => EConstant.From(v)))
                .Or(Tokens.NumberLiteral.Select(v => EConstant.From(v)))
                .Or(Tokens.StringLiteral.Select(v => EConstant.From(v)))
            select constant;

        public static Parser<EObject.Element> ObjectAttribute =>
            from attrName in Tokens.Identifier.Or(Tokens.StringLiteral)
            from _ in Tokens.Colon
            from expr in Expression
            select new EObject.Attribute { Name = attrName, Value = expr };

        public static Parser<EObject.Element> ObjectFragment =>
            from _ in Tokens.DotSpread
            from expr in Expression
            select new EObject.Object { Value = expr };

        public static Parser<EObject.Element> ObjectElement =>
            from element in ObjectAttribute.Or(ObjectFragment)
            select element;

        public static Parser<EObject> Object =>
            from _ in Tokens.CurlyOpen
            from elements in ObjectElement.DelimitedBy(Tokens.Comma)
            from __ in Tokens.CurlyClose
            select new EObject(elements);

        // arrays

        public static Parser<EArray.Element> ArrayItem =>
            from expr in Expression
            select new EArray.ListItem { Value = expr };

        public static Parser<EArray.Element> ArrayFragment =>
            from _ in Tokens.DotSpread
            from expr in Expression
            select new EArray.List { Value = expr };

        public static Parser<EArray.Element> ArrayElement =>
            from element in ArrayItem.Or(ArrayFragment)
            select element;

        public static Parser<EArray> Array_ =>
            from _ in Tokens.SquareBracketOpen
            from elements in ArrayElement.DelimitedBy(Tokens.Comma)
            from __ in Tokens.SquareBracketClose
            select new EArray(elements);

        // functions

        public static Parser<string[]> ClosureWithBrackets =>
            from _ in Tokens.BracketOpen
            from parameters in Tokens.Identifier.DelimitedBy(Tokens.Comma)
            from __ in Tokens.BracketClose
            select parameters.ToArray();

        

        public static Parser<EvalArg> FunctionCallArg =>
            from argName in Tokens.Identifier
            from _ in Tokens.Colon
            from argValue in Expression
            select new EvalArg(argName, argValue);

        static Parser<IEvalExpression> FunctionCall =>
            from funcName in Tokens.Identifier
            from _ in Tokens.BracketOpen
            from args in FunctionCallArg.DelimitedBy(Tokens.Comma)
            from __ in Tokens.BracketClose
            select new ECall(funcName, args);
        
        public static Parser<OpFactory> AndExpression =>
            from _ in Tokens.LogicalAnd
            from right in Expression
            from result in Parsers.Return<OpFactory>(left => new EAnd(left, right))
            select result;

        public static Parser<OpFactory> OrExpression =>
            from _ in Tokens.LogicalOr
            from right in Expression
            from result in Parsers.Return<OpFactory>(left => new EOr(left, right))
            select result;

        public static Parser<OpFactory> ArrayIterator =>
            from _ in Tokens.Dot
            from opName in Tokens.Identifier
            from __ in Tokens.BracketOpen
            from c in DataFuncP
            from ___ in Tokens.BracketClose
            from result in Parsers.Return<OpFactory>(left => GetExpressionFactory(opName)(left, c))
            select result;
        
        public static Parser<OpFactory> PropertyAccess =>
            from _ in Tokens.Dot
            from idn in Tokens.Identifier
            from result in Parsers.Return<OpFactory>(left => new EPath(left, PayloadPath.Parse($"$.{idn}")))
            select result;

        // Data Expression

        public static Parser<IEvalExpression> Expression =>

            // Left-hand side
            from left in Constant
                .Or(from _ in Tokens.BracketOpen
                    from e in Expression
                    from __ in Tokens.BracketClose select e)
                .Or(from fc in FunctionCall select fc)
                .Or(from idn in Tokens.Identifier
                    from result in Parsers.Return(new EVar(idn))
                    select result)
                .Or(Object)
                .Or(Array_)

            // zero or more to the right
            from right in
                 OrExpression
                .Or(AndExpression)
                .Or(ArrayIterator)
                .Or(PropertyAccess)
                .ZeroOrMore()

            // right-hand side expression factories
            from result in Parsers.Return(right.Aggregate(left, (expr, fn) => fn(expr)))

            // final output
            select result;

        // Data Func

        public static Parser<DataFunc> DataFuncP =>
            from parameters in (from idn in Tokens.Identifier select new[] { idn }).Or(ClosureWithBrackets)
            from _ in Tokens.Lambda
            from expr in Expression
            select new DataFunc(expr, parameters);
        
    }
}
