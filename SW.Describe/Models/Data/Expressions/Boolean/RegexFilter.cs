
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SW.Describe.Models
{
    public class RegexFilter : ContentFilterBase
    {
        public override ContentFilterType Type => ContentFilterType.Regex;

        public IContentExpression Value { get; }

        public Regex Regex { get; }

        public override bool IsMatch(IPayload document)
        {

            var leftIssue = Value.TryEvaluate(document, out IPayload left);
            if (leftIssue != null) return false;
            
            if (left is EText text)
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
