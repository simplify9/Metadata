using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.UnitTests.models
{   
    public class AuditDto
    {
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
    
}
