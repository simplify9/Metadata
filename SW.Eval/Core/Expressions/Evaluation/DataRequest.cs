using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval
{
    public class DataRequest
    {
        public class Query
        {
            public string Id { get; }

            public EvalQueryArgs Args { get; }

            public Query(string id, EvalQueryArgs args)
            {
                Id = id;
                Args = args;
            }
        }

        public string DataFuncName { get; }
        
        public Query[] Queries { get; }
        
        public DataRequest(string dataFuncName, IEnumerable<Query> queries)
        {
            DataFuncName = dataFuncName ?? throw new ArgumentNullException(nameof(dataFuncName));
            Queries = queries?.ToArray() ?? throw new ArgumentNullException(nameof(queries));
        }
        
        public DataRequest(string dataFuncName, string requestId, EvalQueryArgs args)
        {
            if (requestId == null) throw new ArgumentNullException(nameof(requestId));
            if (args == null) throw new ArgumentNullException(nameof(args));
            
            DataFuncName = dataFuncName ?? throw new ArgumentNullException(nameof(dataFuncName));
           
            Queries = new[] { new Query(requestId, args) };
        }
    }
}
