using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Factories
{
    public class FromClrDictionary : IContentFactory
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

        public FromClrDictionary(IContentFactory memberFactory)
        {
            _memberFactory = memberFactory;
        }

        readonly IContentFactory _memberFactory;

        public IContentNode CreateFrom(object obj)
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
            return new ContentObject(e, _memberFactory);
        }
    }
}
