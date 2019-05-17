using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core
{
    public enum ContentFilterType
    {
        And,
        Or,
        EqualTo,
        Contains,
        ContainsWhere,
        NotEqualTo,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        All,
        None
    }
}
