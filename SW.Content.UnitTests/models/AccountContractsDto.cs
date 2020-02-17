using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.UnitTests.models
{
    public class AccountContractDto
    {
        public string Product { get; set; }
        public string ChargeType { get; set; }
        public string Ratesheet { get; set; }
        public bool InActive { get; set; }
    }
}
