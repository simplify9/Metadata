using SW.Content.Schema;
using SW.Content.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Factories
{
    public class FromClrIEnumerable : IContentNodeFactory, IContentSchemaNodeFactory
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

        readonly IContentNodeFactory _itemFactory;
        readonly IContentSchemaNodeFactory _schemaFactory;

        public FromClrIEnumerable(IContentNodeFactory itemFactory, IContentSchemaNodeFactory schemaFactory)
        {
            _itemFactory = itemFactory;
            _schemaFactory = schemaFactory;
        }

        public IContentNode CreateFrom(object obj)
        {
            if (obj is IEnumerable enumerable)
            {
                return new ContentList(enumerable, enumerable, _itemFactory);
            }

            return null;

            //var enumerableType = obj.GetType().GetEnumerableTypeArgument();
            //if (enumerableType == null) return null;
            
            //var args = enumerableType.GetGenericArguments();
            //var proxyType = typeof(Proxy<>).MakeGenericType(enumerableType);
            //var proxy = Activator.CreateInstance(proxyType) as IProxy;
            //var e = proxy.Cast(obj);
            //return new ContentList(e, _itemFactory);
        }

        public IMust CreateSchemaNodeFrom(Type type)
        {
            var enumerableType = type.GetEnumerableTypeArgument();
            if (enumerableType == null) return null;

            return new MustBeList(_schemaFactory.CreateSchemaNodeFrom(enumerableType),
                null, null, new ContentSchemaRule[] { });
        }
    }
}
