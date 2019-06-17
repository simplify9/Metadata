using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Schema
{
    public class MustBeOneOf : IMust
    {
        
        public IEnumerable<IMust> Options { get; }

        public MustBeOneOf(IEnumerable<IMust> options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public IEnumerable<SchemaIssue> FindIssues(IContentNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }
            
            foreach (var option in Options)
            {
                var issues = option.FindIssues(node);
                if (!issues.Any())
                {
                    yield break;
                }
            }

            yield return new SchemaIssue(ContentPath.Root(), 
                $"Value did not match any of the following: {string.Join(",", Options)}");

            yield break;
        }
    }
}
