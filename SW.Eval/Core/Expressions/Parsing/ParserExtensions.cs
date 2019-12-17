using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SW.Eval.Expressions.Parsing
{
    

    public static class ParserExtensions
    {
        

        public static ParserResult<T> Expect<T>(this ParserResult<T> result, TokenType type)
        {
            return result.Then((lhs,remaining) => 
                remaining.First().TokenType != type

                    ? ParserResult.HasError<T>(remaining, 
                        $"Expected {type}, found {remaining.First().TokenType}")

                    : ParserResult.Success(remaining.Skip(1), lhs));
        }
        
        public static ParserResult<T[]> ParseDelimited<T>(this ParserResult<IEvalExpression> result, 
            TokenType delimiter,
            Func<ParserResult<T>, ParserResult<T>> itemParser)
        {
            return result
                
                .Then((_, rem) =>
                {
                    var parsedArgs = ParserResult.Success(rem, Array.Empty<T>());
                    while (rem.Any() && rem.First().TokenType != TokenType.CloseBracket)
                    {
                        var empty = ParserResult.None<T>(rem);
                        // expect comma for non-first argument
                        if (parsedArgs.Value.Length > 0) empty = empty.Expect(TokenType.Comma);
                        // parse arg
                        var parsedRight = itemParser(empty);

                        // TODO terminate ??

                        // append to args
                        parsedArgs = parsedArgs.Append(parsedRight);
                    }

                    return parsedArgs;
                });
        }

        public static ParserResult<T[]> Append<T>(this ParserResult<T[]> left, ParserResult<T> right)
        {
            //return left.Then(array => )

            if (left.Error != null) return ParserResult.HasError<T[]>(left.RemainingTokens, left.Error);
            if (right.Error != null) return ParserResult.HasError<T[]>(right.RemainingTokens, right.Error);



            var concated = left.Value.Concat(new[] { right.Value }).ToArray();
            return ParserResult.Success(right.RemainingTokens, concated);
        }

        public static ParserResult<IEvalExpression> ExpectLeft(this ParserResult<IEvalExpression> result)
        {
            return result.Then((lhs, remaining) => lhs != null
                ? ParserResult.HasError<IEvalExpression>(remaining, $"Unexpected {remaining.First()}")
                : ParserResult.None<IEvalExpression>(remaining));
        }

        public static ParserResult<IEvalExpression> ExpectRight(this ParserResult<IEvalExpression> result)
        {
            return result.Then((lhs, remaining) => lhs == null
                ? ParserResult.HasError<IEvalExpression>(remaining, $"Unexpected {remaining.First()}")
                : ParserResult.Success(remaining, lhs));
        }

        public static ParserResult<IEvalExpression> Parse(this ParserResult<IEvalExpression> result, Func<DslToken,IEvalExpression> func)
        {
            return result.Then((lhs, remaining) =>
                ParserResult.Success(remaining.Skip(1), func(remaining.First())));
        }

        public static ParserResult<IEvalExpression> ExpectObject(this ParserResult<IEvalExpression> result)
        {
            return result
                // {
                .Expect(TokenType.OpenCurly)
                // command-delimited object elements ... etc.
                .ParseDelimited<EObject.Element>(TokenType.Comma,
                    init => init
                        // ...[E]
                        .ParseIf(t => t.TokenType == TokenType.SpreadDots,
                            s => s.Expect(TokenType.SpreadDots)
                                .Then((_, rem) => rem.ParseExpressionTree())
                                .Then(left => new EObject.Object
                                    {
                                        Value = left
                                    } as EObject.Element))

                        .ParseIf(t => t.TokenType == TokenType.Identifier,
                            s => s
                                // [arg name]
                                .Expect(t => t.TokenType == TokenType.Identifier, idn => idn.Value)
                                // :
                                .Expect(TokenType.Colon)
                                // [E]
                                .Then((left, remaining) =>
                                    remaining.ParseExpressionTree()
                                        .Then(right =>
                                            new EObject.Attribute
                                            {
                                                Name = left,
                                                Value = right
                                            } as EObject.Element))))
                // }
                .Expect(TokenType.CloseCurly)
                // create expression
                .Then(elements => new EObject(elements) as IEvalExpression);
            
        }

        public static ParserResult<IEvalExpression> ExpectArray(this ParserResult<IEvalExpression> result)
        {
            return result
                // [
                .Expect(TokenType.OpenSquareBracket)
                // command-delimited array elements ... etc.
                .ParseDelimited<EArray.Element>(TokenType.Comma,
                    init => init
                        .ParseIf(t => t.TokenType == TokenType.SpreadDots,
                            // ...[E]
                            s => s.Expect(TokenType.SpreadDots)
                                .Then((_, rem) => rem.ParseExpressionTree())
                                .Then(left => new EArray.List
                                {
                                    Value = left
                                } as EArray.Element),
                            // [E]
                            s => s.Then((_, rem) => rem.ParseExpressionTree())
                                .Then(left => new EArray.ListItem
                                {
                                    Value = left
                                } as EArray.Element)))
                // ]
                .Expect(TokenType.CloseSquareBracket)
                // create expression
                .Then(elements => new EArray(elements) as IEvalExpression);
        }

        public static ParserResult<EvalArg[]> ParseArguments(this ParserResult<IEvalExpression> result)
        {
            return result
                // (
                .Expect(TokenType.OpenBracket)
                // comma-delimited args ... etc.
                .ParseDelimited<EvalArg>(TokenType.Comma,
                    empty => empty
                        // [arg name]
                        .Expect(t => t.TokenType == TokenType.Identifier, idn => idn.Value)
                        // :
                        .Expect(TokenType.Colon)
                        // [E]
                        .Then((left, remaining) =>
                                remaining.ParseExpressionTree()
                                    .Then((right, remainingAfter) =>
                                        ParserResult.Success(remainingAfter, new EvalArg(left, right)))))
                // )
                .Expect(TokenType.CloseBracket);
        }

        public static ParserResult<IEvalExpression> ParseClosureFunc(this ParserResult<IEvalExpression> result)
        {
            //return result.ParseIf(t => t.TokenType == TokenType.Identifier,
            throw new NotImplementedException();  
        }

        public static ParserResult<IEvalExpression> ParseExpressionTree(this IEnumerable<DslToken> source)
        {
            var result = ParserResult.None<IEvalExpression>(source);
            ParserResult<IEvalExpression> lastResult = null;
            while (result != lastResult && result.RemainingTokens.Any())
            {
                lastResult = result;
                result = result.ExpectExpression();
            }

            return result;
        }

        public static ParserResult<IEvalExpression> ExpectExpression(this ParserResult<IEvalExpression> state)
        {
            return state

                .ParseIf(t => t.TokenType == TokenType.DollarSign, s => s.Parse(_ => new EVar("$")))
                .ParseIf(t => t.TokenType == TokenType.Identifier, s => s.Parse(idn => new EVar(idn.Value)))
                .ParseIf(t => t.TokenType == TokenType.Null, s => s.Parse(idn => new EConstant(PayloadNull.Singleton)))
                .ParseIf(t => t.TokenType == TokenType.OpenCurly, s => s.ExpectObject())
                .ParseIf(t => t.TokenType == TokenType.OpenSquareBracket, s => s.ExpectArray())

                .ParseIf(t => t.TokenType == TokenType.String, 
                    s => s.Parse(idn =>
                            new EConstant(new PayloadPrimitive<string>(
                                idn.Value.Substring(1, idn.Value.Length - 2)))))

                .ParseIf(t => t.TokenType == TokenType.TrueLiteral,
                    s => s.Parse(_ => new EConstant(new PayloadPrimitive<bool>(true))))

                .ParseIf(t => t.TokenType == TokenType.FalseLiteral,
                    s => s.Parse(_ => new EConstant(new PayloadPrimitive<bool>(false))))

                .ParseIf(t => t.TokenType == TokenType.DateTime,
                    s => s.Parse(idn =>
                            new EConstant(new PayloadPrimitive<DateTime>(
                                DateTime.Parse(idn.Value, null, DateTimeStyles.RoundtripKind)))))

                .ParseIf(t => t.TokenType == TokenType.Number,
                    s => s.Parse(idn =>
                            new EConstant(new PayloadPrimitive<decimal>(decimal.Parse(idn.Value)))))
                                
                .ParseIf(t => t.TokenType == TokenType.OpenBracket,
                    s => s.Value != null
                        ? s.Value is EVar func
                            ? s.ParseArguments().Then(args => (IEvalExpression)new ECall(func.VarName, args))
                            : null // TODO parse closures
                        : s.Expect(TokenType.OpenBracket)
                            .Then((_, rem) => rem.ParseExpressionTree())
                            .Expect(TokenType.CloseBracket))

                .ParseIf(t => t.TokenType == TokenType.Path, 
                    s => s.ExpectRight()
                            .Parse(idn => new EPath(s.Value, PayloadPath.Parse($"${idn.Value}"))))

                .ParseIf(t => t.TokenType == TokenType.And,
                    s => s.ExpectRight()
                            .ExpectExpression()
                            .Then(right => (IEvalExpression)new EAnd(s.Value, right)))
                
                .ParseIf(t => t.TokenType == TokenType.Or,
                    s => s.ExpectRight()
                            .ExpectExpression()
                            .Then(right => (IEvalExpression)new EOr(s.Value, right)))
            ;
        }

    }
}
