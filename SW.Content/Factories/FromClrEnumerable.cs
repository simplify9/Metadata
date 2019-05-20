using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Factories
{
    public class FromClrEnumerable : IContentFactory
    {
        interface IProxy
        {
            IEnumerable<object> Cast(object source);
        }

        class Proxy<T> : IProxy
        {
            public IEnumerable<object> Cast(object source)
            {
                var sourceCast = (IEnumerable<T>)source;
                foreach (var i in sourceCast) yield return i;
            }
        }

        readonly IContentFactory _itemFactory;

        public FromClrEnumerable(IContentFactory itemFactory)
        {
            _itemFactory = itemFactory;
        }

        public IContentNode CreateFrom(object obj)
        {
            var enumerableType = obj.GetType().GetEnumerableTypeArgument();
            if (enumerableType == null) return null;

            //var args = enumerableType.GetGenericArguments();
            var proxyType = typeof(Proxy<>).MakeGenericType(enumerableType);
            var proxy = Activator.CreateInstance(proxyType) as IProxy;
            var e = proxy.Cast(obj);
            return new ContentList(e, _itemFactory);
        }
    }
}
