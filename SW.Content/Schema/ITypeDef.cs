using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Schema
{
    public interface ITypeDef
    {
        
        IEnumerable<SchemaIssue> FindIssues(IContentNode node);
    }
}
