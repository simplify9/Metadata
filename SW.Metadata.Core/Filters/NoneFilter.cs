using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core
{
    public class NoneFilter : IContentFilter, IEquatable<NoneFilter>
    {
        public ContentFilterType Type => ContentFilterType.None;

        public override bool Equals(object obj)
        {
            return Equals(obj as NoneFilter);
        }

        public bool Equals(NoneFilter other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return 2049151605 + Type.GetHashCode();
        }

        public bool IsMatch(IContentNode document)
        {
            return false;
        }

        public override string ToString()
        {
            return "NONE";
        }

        public static bool operator ==(NoneFilter expression1, NoneFilter expression2)
        {
            return EqualityComparer<NoneFilter>.Default.Equals(expression1, expression2);
        }

        public static bool operator !=(NoneFilter expression1, NoneFilter expression2)
        {
            return !(expression1 == expression2);
        }
    }
}
