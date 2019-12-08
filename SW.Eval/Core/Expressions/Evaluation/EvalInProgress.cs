using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SW.Eval
{
    public class EvalInProgress : IEvalState
    {
        public DataRequest[] ReadyToRun { get; }

        EvalInProgress Append(params DataRequest[] more)
        {
            return new EvalInProgress(ReadyToRun.Concat(more));
        }

        public EvalInProgress(DataRequest readyToRun)
        {
            ReadyToRun = new[] { readyToRun };
        }

        public EvalInProgress(IEnumerable<DataRequest> readyToRun)
        {
            ReadyToRun = readyToRun.ToArray();
        }
        
        public IEvalState MergeWith(IEvalState other)
            => other is EvalComplete
                ? this
                : other is EvalInProgress inProgress
                    ? Append(inProgress.ReadyToRun)
                    : throw new NotSupportedException("Merge not supported");
    }
}
