using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public class DataResponse
    {
        public string RequestId { get; }

        public IPayload Data { get; }

        public DataResponse(string requestId, IPayload data)
        {
            RequestId = requestId ?? throw new ArgumentNullException(nameof(requestId));
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }
    }
}
