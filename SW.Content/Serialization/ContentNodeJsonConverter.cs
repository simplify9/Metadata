using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Serialization
{
    public class ContentNodeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(IContentNode));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jToken = JToken.Load(reader);
            return ContentFactory.Default.CreateFrom(jToken);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            (value as IContentNode).ToJson().WriteTo(writer);
        }
    }
}
