using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core
{
    public enum DocumentFilterType
    {
        And,
        Or,
        EqualTo,
        Contains,
        NotEqualTo,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        All,
        None
    }
}
