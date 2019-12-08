using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public class PathExpression : IEvalExpression, IEquatable<PathExpression>
    {
        public IEvalExpression Scope { get; private set; }

        public PayloadPath Path { get; private set; }

        public PathExpression(IEvalExpression scope, PayloadPath path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }
        
        public override string ToString() => Path.ToString();
        
        public override bool Equals(object obj) => Equals(obj as PathExpression);
        

        public bool Equals(PathExpression other)
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

        public IEnumerable<IEvalExpression> GetChildren()
        {
            yield return Scope;
        }

        public IEvalState ComputeState(EvalContext ctx)
            => ctx.Apply(args => 
                new EvalComplete(args[Scope].ValueOf(Path)));

        public static bool operator ==(PathExpression expression1, PathExpression expression2)
        {
            return EqualityComparer<PathExpression>.Default.Equals(expression1, expression2);
        }

        public static bool operator !=(PathExpression expression1, PathExpression expression2)
        {
            return !(expression1 == expression2);
        }
    }
}
