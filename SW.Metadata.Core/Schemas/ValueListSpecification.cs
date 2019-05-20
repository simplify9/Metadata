using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core.Schemas
{
    public class ValueListSpecification : IValueSpecification
    {
        public IValueSpecification ItemSpecification { get; private set; }

        public int? MinItemCount { get; private set; }

        public int? MaxItemCount { get; private set; }

        public bool IsMatch(IContentNode value)
        {
            if (value is ContentList listValue)
            {

                
            }

            return false;
        }
    }
}
