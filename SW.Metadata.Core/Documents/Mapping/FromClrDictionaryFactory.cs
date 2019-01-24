using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core.Mapping
{
    public class FromClrDictionaryFactory : IDocumentValueFactory
    {
        interface IProxy
        {
            IEnumerable<KeyValuePair<string, object>> Cast(object source);
        }

        class Proxy<TKey, TValue> : IProxy
        {
            public IEnumerable<KeyValuePair<string, object>> Cast(object source)
            {
                var sourceCast = (IEnumerable<KeyValuePair<TKey, TValue>>)source;
                foreach (var i in sourceCast)
                {
                    yield return new KeyValuePair<string, object>(i.Key.ToString(), i.Value);
                }
            }
        }
        
        public IDocumentValue CreateFrom(object obj)
        {
            var enumerableType = obj.GetType().GetEnumerableTypeArgument();
            if (enumerableType == null) return null;
            if (!enumerableType.IsGenericType ||
                !enumerableType.GetGenericTypeDefinition()
                    .Equals(typeof(KeyValuePair<,>)))
            {
                return null;
            }

            var proxyType = typeof(Proxy<,>).MakeGenericType(enumerableType.GetGenericArguments());
            var proxy = Activator.CreateInstance(proxyType) as IProxy;
            var e = proxy.Cast(obj);
            return new DocumentKeyValueContainer(e);
        }
    }
}
