using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SW.Eval
{
    public delegate Task<DataResponse[]> EvalDataTaskFactory(DataRequest dataRequest); 
}
