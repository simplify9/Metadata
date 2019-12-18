using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public class ENotImplemented : IEvalExpression
    {
        public string Name { get; }

        public ENotImplemented(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public IEnumerable<EvalArg> GetArgs()
        {
            yield break;
        }

        public EvalStateMapper GetMapper() =>
            (ctx, args) =>
                new EvalComplete(new PayloadError($"Iterator with name '{Name}' does not exist"));
    }
}
