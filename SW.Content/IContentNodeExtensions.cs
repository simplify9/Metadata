using SW.Content.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content
{
    public static class IContentNodeExtensions
    {
        static IEnumerable<ContentPathValue> ProcessVisit(this IContentNode n, ListNode<object> branch)
        {
            yield return new ContentPathValue(ContentPath.Root, n);

            if (n is IRawValueWrapper wrapper)
            {
                branch = branch.Append(wrapper.RawValue);
            }
            
            if (n is ContentObject o)
            {
                var descendents = o
                    .Where(prop => !(prop.Value is IRawValueWrapper w && branch.Contains(w.RawValue)))
                    .SelectMany(prop => 
                        prop.Value.ProcessVisit(branch)
                            .Select(subProp => 
                                new ContentPathValue(prop.Path.Append(subProp.Path), subProp.Value)));

                foreach (var i in descendents) yield return i;
            }

            else if (n is ContentList list)
            {
                var descendents = list.SelectMany((item, idx) =>
                    item.ProcessVisit(branch).Select(pair => 
                        new ContentPathValue(
                            ContentPath.Root.Append(idx).Append(pair.Path),
                            pair.Value)));

                foreach (var i in descendents) yield return i;
            }
        }

        public static IEnumerable<ContentPathValue> Visit(this IContentNode n)
        {
            return ProcessVisit(n, ListNode<object>.Empty);
        }


        public static ContentType ContentType(this IContentNode data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            
            switch (data)
            {
                case ContentNull _: return Content.ContentType.Null;
                case ContentNumber _: return Content.ContentType.Number;
                case ContentText _: return Content.ContentType.Text;
                case ContentDateTime _: return Content.ContentType.DateTime;
                case ContentBoolean _: return Content.ContentType.Boolean;
                case ContentList _: return Content.ContentType.List;
                case ContentObject _: return Content.ContentType.Object;
                case ContentEntity _: return Content.ContentType.Entity;
                default: throw new NotImplementedException("No mapping");
            }
        }
    }
}
