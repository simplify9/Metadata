using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval
{
    public class EPath : IEvalExpression, IEquatable<EPath>
    {
        public IEvalExpression Scope { get; private set; }

        public PayloadPath Path { get; private set; }

        public EPath(IEvalExpression scope, PayloadPath path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }
        
        public override string ToString() => $"{Scope}{Path.ToString().Substring(1)}";
        
        public override bool Equals(object obj) => Equals(obj as EPath);
        
        public bool Equals(EPath other)
        {
            return other != null &&
                   Scope.Equals(other.Scope) &&
                   Path.Equals(other.Path);
        }

        public override int GetHashCode()
        {
            var hashCode = -1188344167;
            hashCode = hashCode * -1521134295 + EqualityComparer<IEvalExpression>.Default.GetHashCode(Scope);
            hashCode = hashCode * -1521134295 + EqualityComparer<PayloadPath>.Default.GetHashCode(Path);
            return hashCode;
        }

        public IEnumerable<EvalArg> GetArgs()
        {
            yield return new EvalArg("scope", Scope);
        }

        public EvalStateMapper GetMapper() => (ctx, args) => new EvalComplete(args.First().ValueOf(Path));
        
        public static bool operator ==(EPath expression1, EPath expression2)
        {
            return EqualityComparer<EPath>.Default.Equals(expression1, expression2);
        }

        public static bool operator !=(EPath expression1, EPath expression2)
        {
            return !(expression1 == expression2);
        }
    }
}
