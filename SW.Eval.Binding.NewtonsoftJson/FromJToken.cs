using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SW.Eval.Binding.NewtonsoftJson
{


    public class FromJToken : IPayloadTypeReaderFactory
    {
        public PayloadTypeReader<T> GetReader<T>()
        {
            if (!typeof(JToken).IsAssignableFrom(typeof(T))) return null;

            var m = typeof(FromJToken)
                .GetMethod(nameof(Read), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(typeof(T));

            return (PayloadTypeReader<T>)Delegate.CreateDelegate(typeof(PayloadTypeReader<T>), m);
        }

        

        static IPayload Read<T>(PayloadReaderContext ctx, T source)
            where T : JToken
        {
            var jToken = source as JToken;

            if (jToken.Type == JTokenType.Null) return PayloadNull.Singleton;

            if (jToken is JObject jObject)
            {
                var jProps = jObject as IEnumerable<KeyValuePair<string, JToken>>;

                return PayloadObject.Combine(jProps
                    .Select(pair =>
                        new KeyValuePair<PayloadPath, IPayload>(
                            PayloadPath.Root.Append(pair.Key), 
                            ctx.Read(pair.Value))));
            }

            else if (jToken is JArray jArray)
            {
                return PayloadArray.Combine(jArray.Select(e => ctx.Read(e)));
            }

            else if (jToken is JValue jValue)
            {
                switch (jValue.Type)
                {
                    case JTokenType.Boolean: return new PayloadPrimitive<bool>(jValue.Value<bool>());

                    case JTokenType.Integer: return new PayloadPrimitive<int>(jValue.Value<int>());

                    case JTokenType.Float: return new PayloadPrimitive<decimal>(jValue.Value<decimal>());

                    case JTokenType.Date: return new PayloadPrimitive<DateTime>(jValue.Value<DateTime>());

                    case JTokenType.String: return new PayloadPrimitive<string>(jValue.Value<string>());
                }
            }

            return new PayloadError($"JValue of type '{jToken}' is not supported");
        }
    }
}
