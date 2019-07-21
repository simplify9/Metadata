using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Schema
{
    public interface IMust
    {
        
        bool TryGetSchema(ContentPath path, out IMust schema); 

        IEnumerable<SchemaIssue> FindIssues(IContentNode node);
    }
}
