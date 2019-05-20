using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Schema
{
    public class MustBeOneOf : IMust
    {
        readonly IEnumerable<IMust> _options;

        public MustBeOneOf(IEnumerable<IMust> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public IEnumerable<SchemaIssue> FindIssues(IContentNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }
            
            foreach (var option in _options)
            {
                var issues = option.FindIssues(node);
                if (!issues.Any())
                {
                    yield break;
                }
            }

            yield return new SchemaIssue(ContentPath.Root(), 
                $"Value did not match any of the following: {string.Join(",", _options)}");

            yield break;
        }
    }
}
