using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content
{
    public interface ISpecification<in TContent>
    {
        bool IsMatch(TContent document);
    }
}
