using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SW.Eval
{
    using ResultMap = IReadOnlyDictionary<string, IPayload>;

    public class EvalContext
    {
        const string ROOTNAME = "~";

        static readonly ResultMap noResults = new Dictionary<string, IPayload>();

        readonly string nameToken;

        
        // sub-context or same-level context
        EvalContext(EvalContext parent, string nameToken, LexicalScope scopeVars, ResultMap materialized)
        {
            Parent = parent;
            this.nameToken = nameToken;
            ScopeVars = scopeVars;
            MaterializedPayloads = materialized;
            Id = parent == null? ROOTNAME : $"{parent.Id}/{nameToken}";
        }

        public ResultMap MaterializedPayloads { get; } = noResults;

        public EvalContext Parent { get; }

        public LexicalScope ScopeVars { get; }

        public string Id { get; }

        public EvalContext(LexicalScope scopeVars)
        {
            ScopeVars = scopeVars ?? throw new ArgumentNullException(nameof(scopeVars));
            nameToken = ROOTNAME;
            Id = nameToken;
        }

        public EvalContext CreateSub(string nameToken)
        {
            if (nameToken == null) throw new ArgumentNullException(nameof(nameToken));

            return new EvalContext(this, nameToken, ScopeVars, MaterializedPayloads);
        }

        public EvalContext WithVariable(string name, IPayload value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (value == null) throw new ArgumentNullException(nameof(value));

            // clone except scope vars
            return new EvalContext(
                Parent,
                nameToken,
                ScopeVars.SetVariable(name, value), 
                MaterializedPayloads);
        }

        public EvalContext Materialize(DataResponse response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            var newResults = MaterializedPayloads.ToDictionary(k => k.Key, v => v.Value);
            newResults[response.RequestId] = response.Data;

            // clone except materialized payloads
            return new EvalContext(
                Parent,
                nameToken, 
                ScopeVars, 
                newResults);
        }
        
    }
}
