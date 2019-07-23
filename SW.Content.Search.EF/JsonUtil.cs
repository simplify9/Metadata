using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SW.Content.Search.EF
{
    public static class JsonUtil
    {
        static readonly JsonSerializer jsonSerializer = new JsonSerializer
        {
            Formatting = Formatting.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };


        public static T Deserialize<T>(string data)
        {
            using (var reader = new JsonTextReader(new StringReader(data)))
            {
                return jsonSerializer.Deserialize<T>(reader);
            }
        }

        public static string Serialize(object obj)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                jsonSerializer.Serialize(sw, obj);
                sw.Flush();
            }

            return sb.ToString();
        }
    }
}
