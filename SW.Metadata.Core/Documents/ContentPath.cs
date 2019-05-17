using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Metadata.Core
{
    public sealed class ContentPath : IEquatable<ContentPath>
    {
        static readonly string[] _root = { };

        readonly string[] _nodes;
        readonly string _concated;
        
        public IEnumerable<string> Nodes => _nodes;

        public static bool TryParse(string representation, out ContentPath path)
        {
            if (representation == null) throw new ArgumentNullException(nameof(representation));
            
            path = new ContentPath(representation.Split('.'));
            return true;
        }

        public static ContentPath Root()
        {
            return new ContentPath(_root);
        }

        public static ContentPath Parse(string representation)
        {
            if (!TryParse(representation, out ContentPath path))
            {
                throw new FormatException($"Input '{representation}' is not a valid document path");
            }

            return path;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ContentPath);
        }

        public bool Equals(ContentPath other)
        {
            return other != null && _concated == other._concated;
        }

        public override int GetHashCode()
        {
            return _concated.GetHashCode();
        }
        
        public ContentPath(IEnumerable<string> nodes)
        {
            if (nodes == null) throw new ArgumentNullException(nameof(nodes));

            _nodes = nodes.ToArray();
            _concated = string.Join(".", nodes);
        }

        public override string ToString()
        {
            return _concated;
        }

        public ContentPath Sub(int skip, int? take = null)
        {
            return new ContentPath(_nodes
                .Skip(skip)
                .Take(take ?? (_nodes.Length - skip)));
        }
    }
}
