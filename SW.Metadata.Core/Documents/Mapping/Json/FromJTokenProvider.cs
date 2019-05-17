using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace SW.Metadata.Core.Mapping
{
    public class FromJTokenProvider : IContentReader
    {
        static readonly IContentNode _null = new ContentNull();

        static readonly Regex _dateTimeRegex = 
            new Regex($"^{ContentDateTime.Regex}$", RegexOptions.Compiled);

        public IContentNode CreateFrom(ContentReaderMap r, object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            if (!(obj is JToken token)) return null;
            
            if (token.Type == JTokenType.Null) return new ContentNull();
            
            if (token is JObject jObject)
            {
                var props = jObject as IEnumerable<KeyValuePair<string, JToken>>;
                return new ContentObject(props
                    .Select(pair => 
                        new KeyValuePair<string, object>(pair.Key, pair.Value)),
                        r);
            }

            if (token is JArray jArray)
            {
                return new ContentList(jArray, r);
            }

            if (token is JValue jValue)
            {
                switch (jValue.Type)
                {
                    case JTokenType.Boolean:
                        return new ContentBoolean(jValue.Value<bool>());

                    case JTokenType.Integer:
                    case JTokenType.Float:
                        return new ContentNumber(jValue.Value<decimal>());

                    case JTokenType.Date:
                        return new ContentDateTime(jValue.Value<DateTime>());

                    case JTokenType.String:
                        var stringValue = jValue.Value<string>();
                        return _dateTimeRegex.IsMatch(stringValue)
                            ? new ContentDateTime(DateTime.Parse(stringValue, null, DateTimeStyles.RoundtripKind))
                            : (IContentNode)new ContentText(stringValue);
                }
            }

            throw new InvalidOperationException("Unidentified JToken");
        }
        
    }
}
