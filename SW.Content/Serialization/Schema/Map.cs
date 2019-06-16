using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Serialization.Schema
{
    static class Map
    {
        public static readonly Dictionary<string, Type> NodesByName =
            new Dictionary<string, Type>
            {
                { "unrecognized", typeof(UnrecognizedNodeDto) },
                { "any", typeof(AnyDto) },
                { "list", typeof(ListDto) },
                { "null", typeof(NullDto) },
                { "object", typeof(ObjectDto) },
                { "one_of", typeof(OneOfDto) },
                { "boolean", typeof(BooleanDto) },
                { "number", typeof(NumberDto) },
                { "text", typeof(TextDto) },
                { "date_time", typeof(DateTimeDto) },
                { "entity", typeof(EntityDto) }
            };

    }
}
