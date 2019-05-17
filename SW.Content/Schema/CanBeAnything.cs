using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Schema
{
    public class CanBeAnything : IMust
    {
        public IEnumerable<SchemaIssue> FindIssues(IContentNode node)
        {
            yield break;
        }
    }
}
