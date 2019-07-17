using Newtonsoft.Json.Linq;
using SW.Content.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content
{
    public static class IContentNodeJsonExtensions
    {
        static JToken CreateJToken(this IContentNode n, ListNode<object> branch)
        {
            if (n is IRawValueWrapper wrapper)
            {
                branch = branch.Append(wrapper.RawValue);
            }

            switch (n)
            {
                case ContentNull nil:
                    return JValue.CreateNull();

                case ContentList list:
                    var jArray = new JArray();
                    foreach (var i in list) jArray.Add(i.CreateJToken(branch));
                    return jArray;

                case ContentObject obj:
                    var jObject = new JObject();
                    var objPairs = obj.Where(prop =>
                        !(prop.Value is IRawValueWrapper w && branch.Contains(w.RawValue)));

                    foreach (var pair in objPairs)
                    {
                        obj.TryEvaluate(pair.Path, out IContentNode result);
                        jObject.Add(pair.Path.Nodes.Last(), result.CreateJToken(branch));
                    }
                    return jObject;

                case ContentBoolean b:
                    return new JValue(b.Value);

                case ContentDateTime dt:
                    return new JValue(dt.Value);

                case ContentNumber num:
                    return new JValue(num.Value);

                case ContentText text:
                    return new JValue(text.Value);

                default:
                    throw new NotSupportedException($"Content of type {n.GetType()} cannot be converted to JSON");
            }
        }

        public static JToken ToJson(this IContentNode contentNode)
        {
            return CreateJToken(contentNode, ListNode<object>.Empty);
        }
    }
}
