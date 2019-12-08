using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public class DataRequest
    {
        public IDataFunc Source { get; }
        
        public IReadOnlyDictionary<string, IPayload> Params { get; }

        public DataRequest(IDataFunc source, IReadOnlyDictionary<string, IPayload> @params)
        {
            Source = source;
            Params = @params;
        }
    }
}
