using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content
{
    public static class IContentNodeExtensions
    {
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
