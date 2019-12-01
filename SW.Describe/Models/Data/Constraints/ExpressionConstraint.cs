
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    
    public class ExpressionConstraint : ITypeConstraintSpecs
    {
        readonly string name;

        ExpressionConstraint()
        {

        }

        public IContentExpression Expression { get; private set; }

        public ExpressionConstraint(string name, IContentExpression expression)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public string GetConstraintName() => Constants.Constraints.Expression;
    }
}
