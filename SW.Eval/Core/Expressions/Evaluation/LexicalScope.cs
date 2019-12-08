using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public class LexicalScope
    {
        LexicalScope parent;
        string varName;
        IPayload value;

        LexicalScope() { }

        public LexicalScope(string varName, IPayload value)
        {
            this.varName = varName ?? throw new ArgumentNullException(nameof(varName));
            this.value = value ?? throw new ArgumentNullException(nameof(value));
        }
        
        public LexicalScope SetVariable(string name, IPayload value)
            => new LexicalScope
            {
                varName = name,
                value = value,
                parent = this
            };
        
        public IPayload GetValue(string varName)
        {
            if (varName == null) throw new ArgumentNullException(nameof(varName));
            if (this.varName == varName) return value;
            if (parent != null) return parent.GetValue(varName);
            return NoPayload.Singleton;
        }
    }
}
