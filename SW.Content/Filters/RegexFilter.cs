using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SW.Content.Filters
{
    public class RegexFilter : IContentFilter
    {
        public ContentFilterType Type => ContentFilterType.Regex;

        public Regex Regex { get; }

        public bool IsMatch(IContentNode document)
        {
            throw new NotImplementedException();
        }

        public RegexFilter(string regex)
        {
            if (regex == null)
            {
                throw new ArgumentNullException(nameof(regex));
            }

            Regex = new Regex(regex, RegexOptions.Compiled);
        }
    }
}
