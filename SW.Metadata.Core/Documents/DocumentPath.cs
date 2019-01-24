using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Metadata.Core
{
    public sealed class DocumentPath : IEquatable<DocumentPath>
    {
        static readonly string[] _root = { };

        readonly string[] _nodes;
        readonly string _concated;
        
        public IEnumerable<string> Nodes => _nodes;

        public static bool TryParse(string representation, out DocumentPath path)
        {
            if (representation == null) throw new ArgumentNullException(nameof(representation));
            
            path = new DocumentPath(representation.Split('.'));
            return true;
        }

        public static DocumentPath Root()
        {
            return new DocumentPath(_root);
        }

        public static DocumentPath Parse(string representation)
        {
            if (!TryParse(representation, out DocumentPath path))
            {
                throw new FormatException($"Input '{representation}' is not a valid document path");
            }

            return path;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DocumentPath);
        }

        public bool Equals(DocumentPath other)
        {
            return other != null && _concated == other._concated;
        }

        public override int GetHashCode()
        {
            return _concated.GetHashCode();
        }
        
        public DocumentPath(IEnumerable<string> nodes)
        {
            if (nodes == null) throw new ArgumentNullException(nameof(nodes));

            _nodes = nodes.ToArray();
            _concated = string.Join(".", nodes);
        }

        public override string ToString()
        {
            return _concated;
        }

        public DocumentPath Sub(int skip, int? take = null)
        {
            return new DocumentPath(_nodes
                .Skip(skip)
                .Take(take ?? (_nodes.Length - skip)));
        }
    }
}
