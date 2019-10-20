//using SW.Content.Schema;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace SW.Content.Serialization.Schema
//{
//    public static class IMustExtensions
//    {
//        public static ContentPropertyDto ToDto(this ContentProperty property)
//        {
//            return new ContentPropertyDto
//            {
//                IsRequired = property.IsRequired,
//                Key = property.Key,
//                Value = property.Value.ToDto()
//            };
//        }

//        public static ContentSchemaRuleDto ToDto(this ContentSchemaRule rule)
//        {
//            return new ContentSchemaRuleDto
//            {
//                Name = rule.Name,
//                Filter = rule.Filter.ToString()
//            };
//        }

//        public static ContentSchemaNodeDto ToDto(this ITypeDef n)
//        {
//            switch (n)
//            {
//                case MustBeOneOf oneOf:
//                    return new OneOfDto
//                    {
//                        Options = oneOf.Options.Select(o => o.ToDto()).ToArray()
//                    };

//                case CanBeAnything anything:
//                    return new AnyDto();

//                case MustBeList list:
//                    return new ListDto
//                    {
//                        Rules = list.Rules.Select(r => r.ToDto()).ToArray(),
//                        Item = list.Item.ToDto(),
//                        MinItemCount = list.MinItemCount,
//                        MaxItemCount = list.MaxItemCount
//                    };
                    
//                case MustBeNull mustBeNull:
//                    return new NullDto();

//                case MustBeObject obj:
//                    return new ObjectDto
//                    {
//                        Rules = obj.Rules.Select(r => r.ToDto()).ToArray(),
//                        Properties = obj.Properties.Select(prop => prop.ToDto()).ToArray()
//                    };

//                case MustHaveType<ContentText> mustBeText:
//                    return new TextDto
//                    {
//                        Rules = mustBeText.Rules.Select(r => r.ToDto()).ToArray(),
//                    };

//                case MustHaveType<ContentNumber> mustBeNumber:
//                    return new NumberDto
//                    {
//                        Rules = mustBeNumber.Rules.Select(r => r.ToDto()).ToArray(),
//                    };

//                case MustHaveType<ContentDateTime> mustBeDateTime:
//                    return new DateTimeDto
//                    {
//                        HasDate = mustBeDateTime.HasDate,
//                        HasTime = mustBeDateTime.HasTime,
//                        Rules = mustBeDateTime.Rules.Select(r => r.ToDto()).ToArray(),
//                    };

//                case MustHaveType<ContentBoolean> mustBeBool:
//                    return new BooleanDto
//                    {
//                        Rules = mustBeBool.Rules.Select(r => r.ToDto()).ToArray(),
//                    };

//                case MustBeEntityWithName mustBeEntity:
//                    return new EntityDto
//                    {
//                        Rules = mustBeEntity.Rules.Select(r => r.ToDto()).ToArray(),
//                        Name = mustBeEntity.EntityName
//                    };

//                default:
//                    throw new NotSupportedException();
//            }
//        }

//    }
//}
