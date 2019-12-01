using SW.Describe.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Describe.Models
{
    public class TypeConstraint
    {
        static readonly IContentExpression _always = AllFilter.Create();

        public static TypeConstraint WholeNumber { get; } = new TypeConstraint(WholeNumberConstraint.Create());

        protected TypeConstraint()
        {

        }

        public string Type { get; private set; }

        public string Name { get; private set; }

        public IContentExpression Condition { get; private set; }

        public ITypeConstraintSpecs Params { get; private set; }

        public TypeConstraint(ITypeConstraintSpecs constraintParams, IContentExpression condition = null)
        {
            Params = constraintParams ?? throw new ArgumentNullException(nameof(constraintParams));

            Type = Constants.NameFromType(constraintParams.GetType()) ?? 
                constraintParams.GetType().FullName;

            Name = constraintParams.GetConstraintName();

            Condition = condition ?? _always;
        }
    }
}
