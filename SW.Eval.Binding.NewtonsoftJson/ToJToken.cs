using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SW.Eval.Binding.NewtonsoftJson
{
    public class ToJToken : IPayloadTypeConverterFactory
    {
        public PayloadTypeConverter<TTo> GetConverter<TTo>()
        {
            var type = typeof(TTo);

            if (!typeof(JToken).IsAssignableFrom(type)) return null;

            MethodInfo m = null;
            var self = GetType();
            if (type == typeof(JToken)) m = self.GetMethod(nameof(ConvertToken));
            else if (type == typeof(JObject)) m = self.GetMethod(nameof(ConvertObject));
            else if (type == typeof(JArray)) m = self.GetMethod(nameof(ConvertArray));
            else if (type == typeof(JValue)) m = self.GetMethod(nameof(ConvertValue));
            
            if (m != null)
            {
                return (PayloadTypeConverter<TTo>)Delegate
                    .CreateDelegate(typeof(PayloadTypeConverter<TTo>), m);
            }

            throw new NotSupportedException(
                $"Writing payload to JToken of type ({typeof(TTo)}) is not supported");
        }

        static IPayload<JObject> ConvertObject<TFrom>(PayloadConversionContext ctx, IPayload<TFrom> payload)
        {
            if (!(payload is IObject)) return new PayloadError<JObject>($"Could not convert to JObject. Payload type mismatch ({payload.GetType()})");

            var jObject = new JObject();
            foreach (var pair in payload.ObjectProperties())
            {
                var valuePayload = ConvertToken(ctx, pair.Value as IPayload<TFrom>);
                if (valuePayload is IPayloadError) return valuePayload.Map(v => default(JObject));
                if (valuePayload.Any())
                {
                    jObject.Add(pair.Key, valuePayload.Value);
                }
            }

            return new PayloadPrimitive<JObject>(jObject);
        }

        static IPayload<JValue> ConvertValue<TFrom>(PayloadConversionContext ctx, IPayload<TFrom> payload)
        {
            if (!(payload is IPrimitive)) return new PayloadError<JValue>($"Could not convert to JValue. Payload type mismatch ({payload.GetType()})");
            return new PayloadPrimitive<JValue>(new JValue(payload.Value));
        }

        static IPayload<JArray> ConvertArray<TFrom>(PayloadConversionContext ctx, IPayload<TFrom> payload)
        {
            if (!(payload is ISet)) return new PayloadError<JArray>($"Could not convert to JArray. Payload type mismatch ({payload.GetType()})");

            var jArray = new JArray();
            foreach (var item in payload.ArrayItems())
            {
                var valuePayload = ConvertToken(ctx, item as IPayload<TFrom>);
                if (valuePayload is IPayloadError) return valuePayload.Map(v => default(JArray));
                if (valuePayload.Any()) jArray.Add(valuePayload.Value);
            }

            return new PayloadPrimitive<JArray>(jArray);
        }

        static IPayload<JToken> ConvertToken<TFrom>(PayloadConversionContext ctx, IPayload<TFrom> payload)
        {
            if (payload is IPayloadError pError) return new PayloadError<JToken>(pError.Error); 
            if (payload is INull) return new PayloadPrimitive<JToken>(JValue.CreateNull());
            if (payload is IObject obj) return ConvertObject(ctx, payload);
            if (payload is ISet) return ConvertArray(ctx, payload);
            if (payload is IPrimitive) return ConvertValue(ctx, payload);
            return new PayloadError<JToken>($"Unsupported payload type ({payload.GetType()})");
        }
        
    }
}
