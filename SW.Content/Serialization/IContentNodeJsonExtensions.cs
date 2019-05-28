using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content
{
    public static class IContentNodeJsonExtensions
    {
        public static JToken ToJson(this IContentNode contentNode)
        {
            switch (contentNode)
            {
                case ContentNull nil:
                    return JValue.CreateNull();

                case ContentList list:
                    var jArray = new JArray();
                    foreach (var i in list.Items) jArray.Add(i.ToJson());
                    return jArray;

                case ContentObject obj:
                    var jObject = new JObject();
                    foreach (var a in obj.Keys)
                    {
                        obj.TryEvaluate(ContentPath.Parse(a), out IContentNode result);
                        jObject.Add(a, result.ToJson());
                    }
                    return jObject;

                case ContentBoolean b:
                    return new JValue(b.Value);

                case ContentDateTime dt:
                    return new JValue(dt.Value);

                case ContentNumber n:
                    return new JValue(n.Value);

                case ContentText text:
                    return new JValue(text.Value);

                default:
                    throw new NotSupportedException($"Content of type {contentNode.GetType()} cannot be converted to JSON");
            }
        }
    }
}
