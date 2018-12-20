using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HWI.Internal.Extensions
{
    public static class TypeExtensions
    {
        public static string StripNamespaces(this Type type)
        {
            if (type.IsGenericType)
            {
                var name = type.Name.Substring(0, type.Name.IndexOf('`'));
                var types = string.Join(",", type.GetGenericArguments().Select(StripNamespaces));
                return $"{name}<{types}>";
            }

            return type.Name;
        }

        public static string GetFriendlyName(this Type contextParentType)
        {
            if (contextParentType == null)
                contextParentType = typeof(object);

            return contextParentType.Namespace + "." + contextParentType.StripNamespaces();
        }
    }

}
