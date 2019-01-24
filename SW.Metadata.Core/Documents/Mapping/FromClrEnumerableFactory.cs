using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core.Mapping
{
    public class FromClrEnumerableFactory : IDocumentValueFactory
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

        public IDocumentValue CreateFrom(object obj)
        {
            var enumerableType = obj.GetType().GetEnumerableTypeArgument();
            if (enumerableType == null) return null;

            var proxyType = typeof(Proxy<>).MakeGenericType(enumerableType.GetGenericArguments());
            var proxy = Activator.CreateInstance(proxyType) as IProxy;
            var e = proxy.Cast(obj);
            return new DocumentValueList(e);
        }
    }
}
