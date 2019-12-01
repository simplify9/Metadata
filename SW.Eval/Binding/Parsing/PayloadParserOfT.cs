using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace SW.Eval.Binding.Parsing
{
    public delegate IPayload<T> ParserFunc<out T>(PayloadReaderContext ctx, string rawData);

    public class PayloadParser<TModel> : IPayloadTypeReaderFactory
    {
        readonly ParserFunc<TModel> rootParser;

        public PayloadParser(ParserFunc<TModel> rootParser)
        {
            this.rootParser = rootParser;
            
        }

        IPayload Parse(PayloadReaderContext ctx, string rawData)
        {
            if (rawData == null) throw new ArgumentNullException(nameof(rawData));
            
            return rootParser(ctx, rawData);
        }
        
        public PayloadTypeReader<T> GetReader<T>()
        {
            if (typeof(T) != typeof(string)) return null;

            return (PayloadTypeReader<T>)Delegate
                .CreateDelegate(typeof(PayloadTypeReader<T>), 
                    typeof(PayloadParser<TModel>)
                        .GetMethod(nameof(Parse), BindingFlags.Instance | BindingFlags.NonPublic));  
        }
    }
}
