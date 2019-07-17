using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Utils
{
    

    public class ListNode<T>
    {
        readonly T _value;
        readonly ListNode<T> _parent;

        ListNode() { }
        
        public static readonly ListNode<T> Empty = new ListNode<T>();

        ListNode(ListNode<T> parent, T value)
        {
            _parent = parent;
            _value = value;
        }

        public ListNode<T> Append(T value)
        {
            return new ListNode<T>(this, value);
        }

        public bool Contains(T o)
        {
            if (this == Empty) return false;
            if (_value != null ^ o != null) return false;
            if (_value == null) return true;
            if (_value.Equals(o)) return true;
            return _parent.Contains(o);
        }
    }
}
