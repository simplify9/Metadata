using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval.Binding.Internal
{
    static class ReflectionUtils
    {
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType
                && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        public static IEnumerable<Type> GetInterfacesRecursive(this Type t)
        {
            var parent = t;
            while (parent != null && parent != typeof(object))
            {
                var interfaces = parent.GetInterfaces();
                foreach (var i in interfaces) yield return i;
                parent = parent.BaseType;
            }
        }

        public static Type[] GetInterfaceTypeArgs(this Type t, Type openGenericInterface)
        {
            if (!openGenericInterface.IsGenericTypeDefinition)
            {
                throw new ArgumentException(
                    $"Type {openGenericInterface} must be a generic type", 
                    nameof(openGenericInterface));
            }

            return GetInterfacesRecursive(t).Where(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition().Equals(openGenericInterface))
                .Select(x => x.GetGenericArguments()).FirstOrDefault() ?? Array.Empty<Type>();
        }

        public static Type GetEnumerableTypeArgument(this Type t, bool includeString = false)
        {
            if (!includeString && t.Equals(typeof(string))) return null;

            return t.GetInterfaceTypeArgs(typeof(IEnumerable<>)).FirstOrDefault();
        }
    }
}
