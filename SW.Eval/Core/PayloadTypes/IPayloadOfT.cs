using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public interface IPayload<out T> : IPayload
    {
        T Value { get; }

        IPayload<K> Map<K>(Func<T, K> map);
    }
}
