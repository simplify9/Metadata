using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Expressions
{
    public class LexicalScope
    {
        readonly ContentObject _data;

        public bool TryEvaluate(ContentPath path, out IContentNode result)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return _data.TryEvaluate(path, out result);
        }

        public LexicalScope(ContentObject data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
        }
    }
}
