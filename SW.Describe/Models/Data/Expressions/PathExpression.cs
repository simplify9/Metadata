using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    public class PathExpression : IContentExpression, IEquatable<PathExpression>
    {
        public IContentExpression Scope { get; private set; }

        public EPath Path { get; private set; }

        public PathExpression(IContentExpression scope, EPath path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public ExpressionIssue TryEvaluate(IPayload input, out IPayload result)
        {
            throw new NotImplementedException();

            //return input.TryEvaluate(Path, out result)
            //    ? null
            //    : new ExpressionIssue($"The expression '{Path}' was not in scope");
        }

        public override string ToString()
        {
            return Path.ToString();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PathExpression);
        }

        public bool Equals(PathExpression other)
        {
            return other != null &&
                   Scope.Equals(other.Scope) &&
                   Path.Equals(other.Path);
        }

        public override int GetHashCode()
        {
            var hashCode = -1188344167;
            hashCode = hashCode * -1521134295 + EqualityComparer<IContentExpression>.Default.GetHashCode(Scope);
            hashCode = hashCode * -1521134295 + EqualityComparer<EPath>.Default.GetHashCode(Path);
            return hashCode;
        }

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
