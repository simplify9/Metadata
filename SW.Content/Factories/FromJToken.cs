using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using SW.Content.Schema;

namespace SW.Content.Factories
{
    public class FromJToken : IContentFactory, IContentSchemaNodeFactory
    {
        static readonly IContentNode _null = new ContentNull();

        static readonly Regex _dateTimeRegex = 
            new Regex($"^{ContentDateTime.Regex}$", RegexOptions.Compiled);

        readonly IContentFactory _contentFactory;

        public FromJToken(IContentFactory contentFactory)
        {
            _contentFactory = contentFactory;
        }

        public IContentNode CreateFrom(object obj)
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
                        _contentFactory);
            }

            if (token is JArray jArray)
            {
                return new ContentList(jArray, _contentFactory);
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
                        return new ContentText(stringValue);
                }
            }

            throw new InvalidOperationException("Unidentified JToken");
        }

        public IMust CreateSchemaNodeFrom(Type type)
        {
            if (type == typeof(JToken)) return new CanBeAnything();
            return null;
        }
    }
}
