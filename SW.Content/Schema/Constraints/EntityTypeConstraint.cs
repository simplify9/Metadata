using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Schema
{
    public class EntityTypeConstraint : IContentSchemaConstraint
    {
        public string TypeName { get; private set; }

        public EntityTypeConstraint(string typeName)
        {
            TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
        }

        public IEnumerable<SchemaIssue> FindIssues(IContentNode node)
        {
            yield break;
        }
    }
}
