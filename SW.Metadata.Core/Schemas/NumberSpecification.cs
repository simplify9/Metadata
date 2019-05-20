using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core.Schemas
{
    public class NumberSpecification : IValueSpecification
    {
        public bool ExcludeMinValue { get; private set; }
        public decimal? MinValue { get; private set; }

        public bool ExcludeMaxValue { get; private set; }
        public decimal? MaxValue { get; private set; }
        
        public bool AllowFractions { get; private set; }

        public bool IsMatch(IContentNode value)
        {
            throw new NotImplementedException();
        }
    }
}
