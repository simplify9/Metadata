using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Serialization.Schema
{
    public class ContentSchemaNodeJsonConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(ContentSchemaNodeDto));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var jToken = serializer.Deserialize<JToken>(reader);
            if (jToken is JObject jObject)
            {
                if (jObject.TryGetValue(nameof(ContentSchemaNodeDto.NodeType), out JToken jNodeType))
                {
                    
                    if (jNodeType is JValue jValue)
                    {
                        
                        var nodeType = jValue.Value?.ToString();
                        if (nodeType != null && Map.NodesByName.TryGetValue(nodeType, out Type leafType))
                        {
                            return jToken.ToObject(leafType, serializer);
                        }
                    }
                }
            }

            return new UnrecognizedNodeDto();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
