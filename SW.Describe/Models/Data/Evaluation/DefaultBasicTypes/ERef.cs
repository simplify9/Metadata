using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    
    public class ERef : EPrimitive<string>
    {
        public string EntityType { get; }

        public ERef(string entityType, string value) : base(value)
        {
            EntityType = entityType;
        }

        public override string ToString() => $"{EntityType}(\"{Value}\")";
        

    }
}
