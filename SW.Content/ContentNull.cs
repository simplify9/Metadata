﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content
{
    public class ContentNull : IContentNode, IContentPrimitive
    {
        static readonly string KEY = "_#%$";

        public ContentNull()
        {

        }
        
        public ComparisonResult CompareWith(IContentNode other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            
            return other is ContentNull
                ? ComparisonResult.EqualTo
                : ComparisonResult.NotEqualTo;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is ContentNull;
        }

        public string CreateMatchKey()
        {
            return KEY;
        }

        public override string ToString()
        {
            return KEY;
        }

        public bool TryEvaluate(ContentPath path, out IContentNode result)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            if (!path.Nodes.Any())
            {
                result = this;
                return true;
            }

            result = null;
            return false;
        }

        public override int GetHashCode()
        {
            return KEY.GetHashCode();
        }
    }
}