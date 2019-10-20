using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Schema
{
    public class PropertyConstraint : IContentSchemaConstraint
    {
        public string PropertyName { get; private set; }

        public ITypeDef PropertyType { get; private set; }

        public bool Required { get; private set; }

        public PropertyConstraint(string propertyName, bool isRequired, ITypeDef propertyType)
        {
            this.PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            this.PropertyType = propertyType ?? throw new ArgumentNullException(nameof(propertyType));
            this.Required = isRequired;
        }

        public IEnumerable<SchemaIssue> FindIssues(IContentNode node)
        {
            var path = ContentPath.Root.Append(PropertyName);
            var exists = node.TryEvaluate(path, out IContentNode result);
            if (exists)
            {
                var fieldIssues = PropertyType.FindIssues(result);
                foreach (var issue in fieldIssues)
                {
                    yield return new SchemaIssue(
                        path.Append(issue.SubjectPath),
                        issue.Error);
                }
            }
            else if (Required)
            {
                yield return new SchemaIssue(path, $"required but missing");
            }
        }
    }
}
