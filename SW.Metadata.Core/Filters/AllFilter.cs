﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core
{
    public class AllFilter : IDocumentFilter, IEquatable<AllFilter>
    {
        public DocumentFilterType Type => DocumentFilterType.All;

        public override bool Equals(object obj)
        {
            return Equals(obj as AllFilter);
        }

        public bool Equals(AllFilter other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return 2049151605 + Type.GetHashCode();
        }

        public bool IsMatch(Document document)
        {
            return true;
        }

        public static bool operator ==(AllFilter expression1, AllFilter expression2)
        {
            return EqualityComparer<AllFilter>.Default.Equals(expression1, expression2);
        }

        public static bool operator !=(AllFilter expression1, AllFilter expression2)
        {
            return !(expression1 == expression2);
        }

        public override string ToString()
        {
            return "ALL";
        }
    }
}
