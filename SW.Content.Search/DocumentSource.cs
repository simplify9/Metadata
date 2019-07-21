using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Search
{
    public class DocumentSource
    {
        public IContentNode Key { get; private set; }

        public DocumentType Type { get; private set; }

        DocumentSource()
        {

        }

        public DocumentSource(DocumentType type, IContentNode key)
        {
            Key = key;
            Type = type;
        }

        public override bool Equals(object obj)
        {
            return (obj != null && obj is DocumentSource source)
                ? Key.Equals(source.Key) && Type.Equals(source.Type)
                : false;
        }

        public override int GetHashCode()
        {
            var hashCode = 207421273;
            hashCode = hashCode * -1521134295 + EqualityComparer<IContentNode>.Default.GetHashCode(Key);
            hashCode = hashCode * -1521134295 + EqualityComparer<DocumentType>.Default.GetHashCode(Type);
            return hashCode;
        }
    }
}
