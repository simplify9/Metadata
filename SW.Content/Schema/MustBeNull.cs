using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Schema
{
    public class MustBeNull : MustHaveType<ContentNull>
    {
        public MustBeNull() : base(new ContentSchemaRule[] { })
        {

        }
    }
}
