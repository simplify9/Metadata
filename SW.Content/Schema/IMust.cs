using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Schema
{
    public interface IMust
    {
        IEnumerable<SchemaIssue> FindIssues(IContentNode node);
    }
}
