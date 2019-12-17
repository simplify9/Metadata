using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval.Core.Expressions.Parsing.Experimental
{
    public class ParserResult<T> : IParserResult<T>
    {
        readonly T _value;
        public T Value => IsSuccessful ? _value : throw new InvalidOperationException("Parse result is an error. Value is not gettable");

        public string Remaining { get; }
        
        public bool IsSuccessful { get; }

        public ParserResult(T value, string remaining, bool isSuccessful)
        {
            if (isSuccessful && value == null) throw new InvalidOperationException("null is not an acceptable successful parser value");
            _value = value;
            Remaining = remaining;
            IsSuccessful = isSuccessful;
        }
    }


}
