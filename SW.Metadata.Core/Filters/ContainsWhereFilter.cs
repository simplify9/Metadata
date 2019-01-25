using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core
{
    public class ContainsWhereFilter : IDocumentFilter
    {
        public DocumentFilterType Type => DocumentFilterType.ContainsWhere;
        
        public DocumentPath ListPath { get; private set; }

        public IDocumentFilter ItemFilter { get; private set; }

        public ContainsWhereFilter(DocumentPath listPath, IDocumentFilter itemFilter)
        {
            ListPath = listPath ?? throw new ArgumentNullException(nameof(listPath));
            ItemFilter = itemFilter ?? throw new ArgumentNullException(nameof(itemFilter));
        }

        public bool IsMatch(Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            
            if (document.TryEvaluate(ListPath, out IDocumentValue list))
            {
                // evaluate filter against each child
                var items = document.AsEnumerable(list);
                foreach (var i in items)
                {
                    var subDocument = document.CreateSub(i);
                    if (ItemFilter.IsMatch(subDocument)) return true;
                }
            }
            
            return false;
        }
    }
}
