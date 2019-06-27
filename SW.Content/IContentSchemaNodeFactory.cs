using SW.Content.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content
{
    public interface IContentSchemaNodeFactory
    {
        IMust CreateSchemaNodeFrom(Type type);
    }
}
