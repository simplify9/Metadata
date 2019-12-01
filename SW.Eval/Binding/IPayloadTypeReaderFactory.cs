using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval.Binding
{
    public delegate IPayload PayloadTypeReader<in T>(PayloadReaderContext ctx, T source);

    public interface IPayloadTypeReaderFactory
    {
        PayloadTypeReader<T> GetReader<T>();
    }
}
