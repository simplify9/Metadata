using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SW.Content
{
    public sealed class ContentPath : IEquatable<ContentPath>
    {
        static readonly Regex _regex = new Regex(@"^\$(\.([a-z]|[A-Z]|_)+([a-z]|[A-Z]|_|[0-9])*)*$");

        static readonly string[] _noNodes = { };

        public static readonly ContentPath Root = new ContentPath();



        readonly string _token;
        readonly ContentPath _parent;


        static IEnumerable<string> SingleValue(string v)
        {
            yield return v;
        }

        ContentPath()
        {
            Length = 0;
        }

        ContentPath(ContentPath parent, string token)
        {
            _parent = parent;
            _token = token;
            Length = 1 + _parent.Length;
        }

        public int Length { get; }

        public static bool TryParse(string representation, out ContentPath path)
        {
            if (representation == null) throw new ArgumentNullException(nameof(representation));
            path = null;
            if (!_regex.IsMatch(representation)) return false;
            if (representation == "$") path = Root;
            else path = Root.Append(representation.Split('.').Skip(1));
            return true;
        }
        
        public static ContentPath Parse(string representation)
        {
            if (!TryParse(representation, out ContentPath path))
            {
                throw new FormatException($"Input '{representation}' is not a valid path");
            }
            return path;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ContentPath);
        }

        public bool Equals(ContentPath other)
        {
            if (other == null) return false;
            if (other._token != _token) return false;
            if (other._parent == null) return _parent == null;
            if (_parent == null) return false;
            return _parent.Equals(other._parent);
        }

        public override int GetHashCode() => ToString().GetHashCode();
        
        public IEnumerable<string> Nodes => 
            _parent == null 
                ? _noNodes 
                : _parent.Nodes.Concat(SingleValue(_token));
        
        
        public ContentPath Append(int index) => Append($"[{index}]");

        public ContentPath Append(ContentPath another) => Append(another.Nodes);

        public ContentPath Append(IEnumerable<string> nodes)
        {
            var p = this;
            foreach (var n in nodes) p = p.Append(n);
            return p;
        }

        public ContentPath Append(string property)
        {
            if (property.Contains("."))
            {
                throw new ArgumentException($"Value {property} cannot contain '.'", nameof(property));
            }

            return new ContentPath(this, property);
        }
        
        public override string ToString() => 
            _parent == null
                ? "$" 
                : $"{_parent}.{_token}";
        
        

        public ContentPath Sub(int skip, int? take = null) => 
            Root.Append(Nodes
                .Skip(skip)
                .Take(take ?? (Length - skip)));
        
    }
}
