using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Expressions
{
    public class LexicalScope
    {
        readonly ContentObject _data;

        public LexicalScope(ContentObject data)
        {
            _data = data;
        }
    }
}
