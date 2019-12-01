using SW.Describe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Describe
{
    public static class Constants
    {
        static readonly Dictionary<string, Type> _map = new Dictionary<string, Type>
        {
            { Constraints.BaseType, typeof(BaseTypeConstraint) },
            { Constraints.DataSource, typeof(DataSourceConstraint) },
            { Constraints.DateOrTime, typeof(DateOrTimeConstraint) },
            { Constraints.EntityType, typeof(EntityTypeConstraint) },
            { Constraints.Expression, typeof(ExpressionConstraint) },
            { Constraints.ListItemCount, typeof(ListItemCountConstraint) },
            { Constraints.ListItemType, typeof(ListItemTypeConstraint) },
            { Constraints.ValueRange, typeof(PrimitiveValueRangeConstraint) },
            { Constraints.PropertyRequired, typeof(PropertyRequiredConstraint) },
            { Constraints.PropertyType, typeof(PropertyTypeConstraint) },
            { Constraints.Regex, typeof(RegexConstraint) },
            { Constraints.WholeNumber, typeof(WholeNumberConstraint) },

            { DataSources.Inline, typeof(InlineDataSource) },
            { DataSources.Remote, typeof(RemoteDataSource) },

            { BasicTypes.Boolean, typeof(EBool) },
            { BasicTypes.DateTime, typeof(EDateTime) },
            { BasicTypes.EntityRef, typeof(ERef) },
            { BasicTypes.Null, typeof(ENull) },
            { BasicTypes.Number, typeof(INumber) },
            { BasicTypes.Object, typeof(EObject) },
            { BasicTypes.Set, typeof(ESet) },
            { BasicTypes.Text, typeof(EText) },
        };

        public static string NameFromType(Type t) => _map.Where(p => p.Value.IsAssignableFrom(t)).Select(p => p.Key).FirstOrDefault();
        
        public static Type TypeFromName(string name) => _map.Where(p => p.Key == name).Select(p => p.Value).FirstOrDefault();

        public static class Constraints
        {
            public const string BaseType = "basetype";

            public const string DataSource = "datasource";

            public const string DateOrTime = "datetime";

            public const string EntityType = "entitytype";

            public const string Expression = "expression";

            public const string WholeNumber = "wholenumber";

            public const string ListItemCount = "itemcount";

            public const string ListItemType = "itemtype";

            public const string ValueRange = "range";

            public const string PropertyRequired = "required";

            public const string PropertyType = "property";

            public const string Regex = "regex";
        }

        public static class BasicTypes
        {
            public const string Text = "text";

            public const string Number = "number";

            public const string Boolean = "boolean";

            public const string DateTime = "datetime";

            public const string EntityRef = "entityref";

            public const string Set = "set";

            public const string Object = "object";

            public const string Null = "null";
        }

        public static class DataSources
        {
            public const string Inline = "inline";

            public const string Remote = "remote";
        }
    }
}
