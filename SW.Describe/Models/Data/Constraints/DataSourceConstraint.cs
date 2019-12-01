
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SW.Describe.Models
{
    
    public class DataSourceConstraint : ITypeConstraintSpecs
    {
        static readonly Dictionary<string, IContentExpression> _noParams = 
            new Dictionary<string, IContentExpression>();

        DataSourceConstraint()
        {

        }

        public IReadOnlyDictionary<string,IContentExpression> Params { get; private set; }

        public IEntityRefDataSource DataSource { get; private set; }

        public DataSourceConstraint(
            IEntityRefDataSource dataSource, 
            IDictionary<string,IContentExpression> dataSourceParams = null)
        {
            DataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
            Params = new ReadOnlyDictionary<string,IContentExpression>(dataSourceParams ?? _noParams);
        }

        public string GetConstraintName() => Constants.Constraints.DataSource;
    }
}
