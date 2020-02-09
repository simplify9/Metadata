using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace SW.Content.Search.EF
{
    public class StringBuilderHelper 
    {
        readonly StringBuilder sb = new StringBuilder();
        readonly List<object> data = new List<object>();
        //private int paramsCounter = 0;


        public StringBuilderHelper()
        {

        }

        // .Append("UPDATE DocTokens SET twet=? WHERE asdf=? AND erge=?", 34, "fasdfasdf", DateTime.UtcNow)
        public StringBuilderHelper Append(string qryText, params object[] args)
        {
            if (qryText is null) throw new ArgumentNullException(nameof(qryText));

            var i = qryText.IndexOf('?');

            //var offset = data.Count;
            var j = 0;
            while (i != -1)
            {
                sb.Append(qryText.Substring(0, i - 1));
                if (j >= args.Length) throw new ArgumentException("Parameter placeholders exceed given values");
                sb.Append($"@p{data.Count}");
                data.Add(args[j]);
                qryText = qryText.Substring(i + 1);
                i = qryText.IndexOf('?');
                ++j;
            }

            sb.Append(qryText);
            sb.Append(";");

            return this;
        }

        public DbCommand CreateCommand(DbConnection dbConn)
        {
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = sb.ToString();
            var paramsCounter = 0;
            foreach (var v in data)
            {
                var p = cmd.CreateParameter();
                p.ParameterName = $"@p{paramsCounter++}";
                p.Value = v;
                cmd.Parameters.Add(p);
            }
            return cmd;
        }
        public void Clear()
        {
            sb.Clear();
        }
    }
}
