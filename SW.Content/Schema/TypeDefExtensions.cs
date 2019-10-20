using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Schema
{
    public static class TypeDefExtensions
    {
        public static TypeDef<T> WithConstraint<T>(this TypeDef<T> def, string name, IContentSchemaConstraint constraint)
            where T : IContentNode
        {
            var constraints = def.Rules;
            var newConstraints = constraints.Concat(new KeyValuePair<string, IContentSchemaConstraint>[]
                {
                    new KeyValuePair<string, IContentSchemaConstraint>(name, constraint)
                });

            return new TypeDef<T>(newConstraints.ToDictionary(k => k.Key, v => v.Value));
        }

        public static TypeDef<ContentObject> WithProperty(this TypeDef<ContentObject> def, 
            string propertyName,
            bool isRequired,
            ITypeDef propertyType)
        {
            return def.WithConstraint($"_prop.{propertyName}",
                new PropertyConstraint(propertyName, isRequired, propertyType));
        }

        public static TypeDef<ContentList> WithItemsOfType(this TypeDef<ContentList> def,
            ITypeDef itemType)
        {
            return def.WithConstraint("_list_item_type", new ListItemTypeConstraint(itemType));
        }


    }
}
