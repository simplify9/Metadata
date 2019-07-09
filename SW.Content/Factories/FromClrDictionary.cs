using SW.Content.Schema;
using SW.Content.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Factories
{
    public class FromClrDictionary : IContentNodeFactory, IContentSchemaNodeFactory
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

        public FromClrDictionary(IContentNodeFactory memberFactory, IContentSchemaNodeFactory schemaFactory)
        {
            _memberFactory = memberFactory;
            _schemaFactory = schemaFactory;
        }

        readonly IContentNodeFactory _memberFactory;
        readonly IContentSchemaNodeFactory _schemaFactory;

        bool Include(Type enumerableType)
        {
            if (enumerableType == null) return false;
            if (!enumerableType.IsGenericType ||
                !enumerableType.GetGenericTypeDefinition()
                    .Equals(typeof(KeyValuePair<,>)) ||
                    // only string keys
                    !enumerableType.GetGenericArguments()[0].Equals(typeof(string)))
            {
                return false;
            }
            return true;
        }

        public IContentNode CreateFrom(object obj)
        {
            var enumerableType = obj.GetType().GetEnumerableTypeArgument();
            if (!Include(enumerableType)) return null;

            var proxyType = typeof(Proxy<,>).MakeGenericType(enumerableType.GetGenericArguments());
            var proxy = Activator.CreateInstance(proxyType) as IProxy;


            var e = proxy.Cast(obj);
            return new ContentObject(e, obj, _memberFactory);
        }

        public IMust CreateSchemaNodeFrom(Type type)
        {
            var enumerableType = type.GetEnumerableTypeArgument();
            if (!Include(enumerableType)) return null;
            return new MustBeObject(
                _schemaFactory.CreateSchemaNodeFrom(enumerableType.GetGenericArguments()[1]), 
                new ContentSchemaRule[] { });
        }
    }
}
