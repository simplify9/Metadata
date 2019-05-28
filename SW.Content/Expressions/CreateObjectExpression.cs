using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Expressions
{
    public class CreateObjectExpression : IContentExpression
    {
        public IReadOnlyDictionary<string, IContentExpression> Attributes { get; }

        public CreateObjectExpression(IReadOnlyDictionary<string,IContentExpression> attributes)
        {
            Attributes = attributes;
        }

        public ExpressionIssue TryEvaluate(LexicalScope scope, out IContentNode result)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            result = null;

            var attributeMap = new Dictionary<string, object>();

            foreach (var a in Attributes)
            {
                var issue = a.Value.TryEvaluate(scope, out IContentNode attributeValue);
                if (issue != null) return issue;
                attributeMap.Add(a.Key, attributeValue);
            }

            result = new ContentObject(attributeMap, ContentFactory.Default);

            return null;
        }
    }
}
