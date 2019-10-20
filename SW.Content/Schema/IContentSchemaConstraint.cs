using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Schema
{
    public interface IContentSchemaConstraint
    {
        

        IEnumerable<SchemaIssue> FindIssues(IContentNode node);
    }
}
