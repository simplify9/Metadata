﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    public class AllFilter : ContentFilterBase, IEquatable<AllFilter>
    {
        public override ContentFilterType Type => ContentFilterType.All;

        static readonly AllFilter _singleton = new AllFilter();

        AllFilter()
        {

        }

        public static AllFilter Create() => _singleton;

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

        public override bool IsMatch(IPayload value)
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
