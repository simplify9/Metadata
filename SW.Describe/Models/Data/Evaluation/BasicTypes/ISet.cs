using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe
{
    public interface ISet
    {
        IEnumerable<IPayload> Items { get; }
    }
}
