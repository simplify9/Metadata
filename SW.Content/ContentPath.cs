using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SW.Content
{
    public sealed class ContentPath : IEquatable<ContentPath>, IEnumerable<ContentPath.INode>
    {
        public interface INode
        {

        }

        public class ObjectProperty : INode
        {
            public string PropertyName { get; }

            public ObjectProperty(string propName)
            {
                PropertyName = propName;
            }

            public override string ToString()
            {
                return PropertyName.ToString();
            }

            public override bool Equals(object obj)
            {
                return obj != null && obj is ObjectProperty p && p.PropertyName == PropertyName;
            }

            public override int GetHashCode()
            {
                return PropertyName.GetHashCode();
            }
        }

        public class AnyListIndex : INode
        {
            public override bool Equals(object obj)
            {
                return obj is AnyListIndex;
            }

            public override int GetHashCode()
            {
                return ToString().GetHashCode();
            }

            public override string ToString()
            {
                return "[]";
            }
        }

        public class ListIndex : INode
        {
            public int Index { get; }

            public ListIndex(int index)
            {
                Index = index;
            }

            public override string ToString()
            {
                return $"[{Index}]";
            }

            public override bool Equals(object obj)
            {
                return obj != null && obj is ListIndex i && i.Index == Index;
            }

            public override int GetHashCode()
            {
                return Index.GetHashCode();
            }
        }

        

        static readonly string _propertyRegex = @"([a-z]|[A-Z]|_)+([a-z]|[A-Z]|_|[0-9])*";
        static readonly string _indexRegex = @"\[[0-9]*\]";
        static readonly Regex _regex = new Regex($@"^\$(\.(({_propertyRegex})|({_indexRegex})))*$", RegexOptions.Compiled);

        static readonly INode[] _noNodes = { };

        public static readonly ContentPath Root = new ContentPath();

        public static readonly AnyListIndex AnyIndex = new AnyListIndex();

        readonly INode _token;
        readonly ContentPath _parent;
        
        ContentPath()
        {
            Length = 0;
        }

        ContentPath(ContentPath parent, INode token)
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
            else path = Root.Append(representation.Split('.').Skip(1)
                .Select(p => p.StartsWith("[")
                    ? (p.Length > 2? (INode)new ListIndex(int.Parse(p.Trim('[', ']'))): AnyIndex)
                    : new ObjectProperty(p)));
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

            if (this == Root ^ other == Root) return false;

            if (this == Root) return true;

            if (!_token.Equals(other._token)) return false;
            return _parent.Equals(other._parent);
        }

        public override int GetHashCode() => ToString().GetHashCode();
        
        //public IEnumerable<string> Nodes => 
        //    _parent == null 
        //        ? _noNodes 
        //        : _parent.Nodes.Concat(SingleValue(_token));
        
        
        public ContentPath Append(int index) => Append(new ListIndex(index));

        //public ContentPath Append(ContentPath another) => Append(another.Nodes);

        public ContentPath Append(IEnumerable<INode> nodes)
        {
            var p = this;
            foreach (var n in nodes) p = p.Append(n);
            return p;
        }

        public ContentPath Append(string property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (property.Contains("."))
            {
                throw new ArgumentException($"Value {property} cannot contain '.'", nameof(property));
            }

            return Append(new ObjectProperty(property));
        }

        public ContentPath Append(INode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            
            return new ContentPath(this, node);
        }
        
        public override string ToString() => 
            _parent == null
                ? "$" 
                : $"{_parent}.{_token}";

        public ContentPath Sub(int skip, int? take = null) => 
            Root.Append(this.Skip(skip)
                .Take(take ?? (Length - skip)));

        public IEnumerator<INode> GetEnumerator()
        {
            if (_parent != null) foreach (var n in _parent) yield return n;
            if (_token != null) yield return _token;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
