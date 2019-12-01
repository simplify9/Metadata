using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SW.Eval.Binding.StandardTypes
{
    using PayloadPair = KeyValuePair<PayloadPath, IPayload>;

    delegate PayloadPair PropertyReader<T>(PayloadReaderContext ctx, T source);

    public class FromClass : IPayloadTypeReaderFactory
    {
        
        public PayloadTypeReader<T> GetReader<T>()
        {
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var propReaders = props.Select(prop => BuildPropertyReader<T>(prop)).ToArray();
            return (ctx, poco) => new PayloadObject(propReaders.Select(r => r(ctx, poco)));
        }
        
        static PropertyReader<T> BuildPropertyReader<T>(PropertyInfo prop)
        {
            var ctxParam = Expression.Parameter(typeof(PayloadReaderContext));
            var sourceParam = Expression.Parameter(typeof(T));
            
            var path = PayloadPath.Root.Append(prop.Name);
            
            var createSubMethod = typeof(PayloadReaderContext)
                .GetMethod(
                    nameof(PayloadReaderContext.CreateSub), 
                    BindingFlags.Public | BindingFlags.Instance);

            var readMethod = typeof(PayloadReaderContext)
                .GetMethod(
                    nameof(PayloadReaderContext.Read),
                    BindingFlags.Public | BindingFlags.Instance)
                .MakeGenericMethod(prop.PropertyType);

            var propValue = Expression.Property(sourceParam, prop);

            var subCtx = Expression.Call(ctxParam, createSubMethod, sourceParam);
            var readExp = Expression.Call(subCtx, readMethod, propValue);

            var ctor = typeof(PayloadPair).GetConstructors().First(c => c.GetParameters().Length == 2);

            var body = Expression.New(ctor, Expression.Constant(path), readExp);

            return Expression.Lambda<PropertyReader<T>>(body, ctxParam, sourceParam).Compile();
        }
        
    }
}
