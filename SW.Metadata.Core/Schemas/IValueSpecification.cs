using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core.Schemas
{
    public interface IValueSpecification
    {
        bool IsMatch(IContentNode value);
    }
}
