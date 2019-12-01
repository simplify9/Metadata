using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SW.Eval
{
    public class PayloadPath : IEquatable<PayloadPath>, IEnumerable<PayloadPath.INode>
    {
        public interface INode { }

        public struct Property : INode
        {
            public string PropertyName { get; }

            public Property(string propName)
            {
                PropertyName = propName;
            }

            public override string ToString() => PropertyName.ToString();
            
            public override bool Equals(object obj) => obj != null && obj is Property p && p.PropertyName == PropertyName;
            
            public override int GetHashCode() => PropertyName.GetHashCode();
        }

        public struct OpenIndex : INode
        {
            public override bool Equals(object obj) => obj is OpenIndex;
            
            public override int GetHashCode() => ToString().GetHashCode();
            
            public override string ToString() => "[]";
        }

        public struct Index : INode
        {
            public int Value { get; }

            public Index(int index)
            {
                Value = index;
            }

            public override string ToString() => $"[{Value}]";
            
            public override bool Equals(object obj) => obj != null && obj is Index i && i.Value == Value;
            
            public override int GetHashCode() => Value.GetHashCode();
        }
        
        static readonly string _propertyRegex = @"([a-z]|[A-Z]|_)+([a-z]|[A-Z]|_|[0-9])*";
        static readonly string _indexRegex = @"\[[0-9]*\]";
        static readonly Regex _regex = new Regex($@"^\$(\.(({_propertyRegex})|({_indexRegex})))*$", RegexOptions.Compiled);

        static readonly INode[] _noNodes = { };

        public static readonly PayloadPath Root = new PayloadPath();

        public static readonly OpenIndex AnyIndex = new OpenIndex();

        readonly INode _token;
        readonly PayloadPath _parent;
        
        PayloadPath()
        {
            Length = 0;
        }

        PayloadPath(PayloadPath parent, INode token)
        {
            _parent = parent;
            _token = token;
            Length = 1 + _parent.Length;
        }

        public int Length { get; }

        public static bool TryParse(string representation, out PayloadPath path)
        {
            if (representation == null) throw new ArgumentNullException(nameof(representation));
            path = null;
            if (!_regex.IsMatch(representation)) return false;
            if (representation == "$") path = Root;
            else path = Root.Append(representation.Split('.').Skip(1)
                .Select(p => p.StartsWith("[")
                    ? (p.Length > 2? (INode)new Index(int.Parse(p.Trim('[', ']'))): AnyIndex)
                    : new Property(p)));
            return true;
        }
        
        public static PayloadPath Parse(string representation)
        {
            if (!TryParse(representation, out PayloadPath path))
            {
                throw new FormatException($"Input '{representation}' is not a valid path");
            }
            return path;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PayloadPath);
        }

        public bool Equals(PayloadPath other)
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
        
        
        public PayloadPath Append(int index) => Append(new Index(index));

        //public ContentPath Append(ContentPath another) => Append(another.Nodes);

        public PayloadPath Append(IEnumerable<INode> nodes)
        {
            var p = this;
            foreach (var n in nodes) p = p.Append(n);
            return p;
        }

        public PayloadPath Append(string property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (property.Contains("."))
            {
                throw new ArgumentException($"Value {property} cannot contain '.'", nameof(property));
            }

            return Append(new Property(property));
        }

        public PayloadPath Append(INode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            
            return new PayloadPath(this, node);
        }
        
        public override string ToString() => 
            _parent == null
                ? "$" 
                : $"{_parent}.{_token}";

        public PayloadPath Sub(int skip, int? take = null) => 
            Root.Append(this.Skip(skip)
                .Take(take ?? (Length - skip)));

        public IEnumerator<INode> GetEnumerator()
        {
            if (_parent != null) foreach (var n in _parent) yield return n;
            if (_token != null) yield return _token;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
