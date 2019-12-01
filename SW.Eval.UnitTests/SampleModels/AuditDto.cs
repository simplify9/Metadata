using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval.UnitTests
{   
    public class AuditDto
    {
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
    
}
