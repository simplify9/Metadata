using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Schema
{
    public class MustBeEntityWithName : MustHaveType<ContentEntity>
    {
        public string EntityName { get; private set; }

        public MustBeEntityWithName(string entityName, IEnumerable<ContentSchemaRule> rules)
            : base(rules)
        {
            EntityName = entityName ?? throw new ArgumentNullException(nameof(entityName));
        }

        public override IEnumerable<SchemaIssue> FindIssues(IContentNode node)
        {
            if (!(node is ContentText))
            {
                var issues = base.FindIssues(node);
                foreach (var i in issues) yield return i;
            }
        }
    }
}
