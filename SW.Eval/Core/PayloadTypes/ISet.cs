﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public interface ISet
    {
        IEnumerable<IPayload> Items { get; }
    }
}
