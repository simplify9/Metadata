using SW.Content.Expressions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SW.Content.Filters
{
    public class RegexFilter : ContentFilterBase
    {
        public override ContentFilterType Type => ContentFilterType.Regex;

        public IContentExpression Value { get; }

        public Regex Regex { get; }

        public override bool IsMatch(IContentNode document)
        {

            var leftIssue = Value.TryEvaluate(document, out IContentNode left);
            if (leftIssue != null) return false;
            
            if (left is ContentText text)
            {
                return Regex.IsMatch(text.Value);
            }

            return false;
        }

        public RegexFilter(IContentExpression value, string regex)
        {


            if (regex == null)
            {
                throw new ArgumentNullException(nameof(regex));
            }

            Value = value ?? throw new ArgumentNullException(nameof(value));
            Regex = new Regex(regex, RegexOptions.Compiled);
        }
    }
}
