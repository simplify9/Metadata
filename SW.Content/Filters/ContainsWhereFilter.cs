using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Filters
{
    public class ContainsWhereFilter : IContentFilter
    {
        public ContentFilterType Type => ContentFilterType.ContainsWhere;
        
        public ContentPath ListPath { get; private set; }

        public IContentFilter ItemFilter { get; private set; }

        public ContainsWhereFilter(ContentPath listPath, IContentFilter itemFilter)
        {
            ListPath = listPath ?? throw new ArgumentNullException(nameof(listPath));
            ItemFilter = itemFilter ?? throw new ArgumentNullException(nameof(itemFilter));
        }

        public bool IsMatch(IContentNode document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            
            if (document.TryEvaluate(ListPath, out IContentNode node)
                && node is ContentList list)
            {
                // evaluate filter against each child
                return list.Items.Any(i => ItemFilter.IsMatch(i));
            }
            
            return false;
        }

        public override string ToString()
        {
            return $"{ListPath} CONTAINS ({ItemFilter})";
        }
    }
}
