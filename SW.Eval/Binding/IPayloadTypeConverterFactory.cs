using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval.Binding
{
    public delegate IPayload<TTo> PayloadTypeConverter<TTo>(PayloadConversionContext ctx, IPayload payload);

    public interface IPayloadTypeConverterFactory
    {
        PayloadTypeConverter<TTo> GetConverter<TTo>();
    }
}
