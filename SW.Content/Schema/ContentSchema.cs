using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Schema
{
    public class ContentSchema
    {
        public IMust Root { get; }
        
        public ContentSchema(IMust root)
        {
            Root = root ?? throw new ArgumentNullException(nameof(root));
        }
        
        public IEnumerable<SchemaIssue> FindIssues(IContentNode value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return Root.FindIssues(value);
        }
    }
}
