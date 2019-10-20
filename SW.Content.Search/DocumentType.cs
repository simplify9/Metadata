using SW.Content.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Search
{
    public class DocumentType
    {
        public string Name { get; private set; }

        public ITypeDef Schema { get; private set; }
        
        public DocumentType(Type fromType)
        {
            Name = fromType.AssemblyQualifiedName;
            Schema = ContentSchemaNodeFactory.Default.CreateSchemaNodeFrom(fromType);
        }
        
        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is DocumentType type && type.Name == Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
