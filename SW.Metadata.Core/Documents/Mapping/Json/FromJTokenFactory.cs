using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace SW.Metadata.Core.Mapping
{
    public class FromJTokenFactory : IDocumentValueFactory
    {
        static readonly IDocumentValue _null = new NullValue();

        static readonly Regex _dateTimeRegex = 
            new Regex($"^{DateTimeValue.Regex}$", RegexOptions.Compiled);

        public IDocumentValue CreateFrom(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            if (!(obj is JToken token)) return null;
            
            if (token.Type == JTokenType.Null) return new NullValue();
            
            if (token is JObject jObject)
            {
                var props = jObject as IEnumerable<KeyValuePair<string, JToken>>;
                return new DocumentKeyValueContainer(props
                    .Select(pair => 
                        new KeyValuePair<string, object>(pair.Key, pair.Value)));
            }

            if (token is JArray jArray)
            {
                return new DocumentValueList(jArray);
            }

            if (token is JValue jValue)
            {
                switch (jValue.Type)
                {
                    case JTokenType.Boolean:
                        return new BooleanValue(jValue.Value<bool>());

                    case JTokenType.Integer:
                    case JTokenType.Float:
                        return new NumericValue(jValue.Value<decimal>());

                    case JTokenType.Date:
                        return new DateTimeValue(jValue.Value<DateTime>());

                    case JTokenType.String:
                        var stringValue = jValue.Value<string>();
                        return _dateTimeRegex.IsMatch(stringValue)
                            ? new DateTimeValue(DateTime.Parse(stringValue, null, DateTimeStyles.RoundtripKind))
                            : (IDocumentValue)new TextValue(stringValue);
                }
            }

            throw new InvalidOperationException("Unidentified JToken");
        }
        
    }
}
