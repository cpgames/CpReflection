using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace cpGames.core.CpReflection
{
    public static class ReflectionCommon
    {
        #region Methods
        /// <summary>
        /// Get element type of a collection.
        /// Unlike default GetElementType, drills into base classes to find first base class that is a collection.
        /// </summary>
        /// <param name="type">Type to search</param>
        /// <returns>Element type if found, otherwise null</returns>
        public static Type GetElementTypeEx(this Type type)
        {
            if (type == null)
            {
                return null;
            }
            if (type.GetElementType() != null)
            {
                return type.GetElementType();
            }
            if (type.GetGenericArguments().Length > 0)
            {
                return type.GetGenericArguments()[0];
            }
            return GetElementTypeEx(type.BaseType);
        }

        /// <summary>
        /// Checks if type is a struct.
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <returns>True if a struct, otherwise False</returns>
        public static bool IsStruct(this Type type)
        {
            return type.IsValueType && !type.IsPrimitive && !type.IsEnum;
        }

        /// <summary>
        /// Find all derived types of a base class.
        /// </summary>
        /// <param name="type">Type of base class to search</param>
        /// <param name="assembly">Assembly to search (if null use type's assembly)</param>
        /// <param name="includeSelf">Include searched type in return value</param>
        /// <param name="includeAbstract">Include abstract types in return value</param>
        /// <returns>Enumeration of all derived types</returns>
        public static IEnumerable<Type> FindAllDerivedTypes(this Type type, Assembly assembly = null, bool includeSelf = false, bool includeAbstract = false)
        {
            if (assembly == null)
            {
                assembly = Assembly.GetAssembly(type);
            }
            return assembly
                .GetTypes()
                .Where(t =>
                    (includeSelf || t != type) &&
                    (includeAbstract || !t.IsAbstract) &&
                    type.IsAssignableFrom(t));
        }

        /// <summary>
        /// Find all derived types of a base class (generic).
        /// </summary>
        /// <typeparam name="T">Type of base class to search</typeparam>
        /// <param name="assembly">Assembly to search (if null use type's assembly)</param>
        /// <param name="includeSelf">Include searched type in return value</param>
        /// <param name="includeAbstract">Include abstract types in return value</param>
        /// <returns>Enumeration of all derived types</returns>
        public static IEnumerable<Type> FindAllDerivedTypes<T>(Assembly assembly = null, bool includeSelf = false, bool includeAbstract = false)
        {
            return FindAllDerivedTypes(typeof(T), assembly, includeSelf, includeAbstract);
        }

        /// <summary>
        /// Check if type is derived from base type or is base type.
        /// </summary>
        /// <param name="derivedType">Type to check</param>
        /// <param name="baseType">Base type</param>
        /// <returns>True of derived, else False</returns>
        public static bool IsTypeOrDerived(this Type derivedType, Type baseType)
        {
            return baseType == derivedType ||
                derivedType.IsSubclassOf(baseType) ||
                baseType.IsAssignableFrom(derivedType);
        }

        /// <summary>
        /// Check if instance type is derived from base instance type or is base instance type.
        /// </summary>
        /// <param name="baseObj">Base object instance</param>
        /// <param name="derivedObj">Derived object instance</param>
        /// <returns>True of derived, else False</returns>
        public static bool IsTypeOrDerived(object baseObj, object derivedObj)
        {
            return derivedObj.GetType().IsTypeOrDerived(baseObj.GetType());
        }

        /// <summary>
        /// heck if instance type is derived from base type or is base type.
        /// </summary>
        /// <param name="baseType">Base type</param>
        /// <param name="derivedObj">Derived object instance</param>
        /// <returns>True of derived, else False</returns>
        public static bool IsTypeOrDerived(Type baseType, object derivedObj)
        {
            return derivedObj.GetType().IsTypeOrDerived(baseType);
        }

        /// <summary>
        /// Invoke static generic method in a class.
        /// </summary>
        /// <param name="objectType">Object type containing the method to invoke</param>
        /// <param name="methodName">Method name to invoke</param>
        /// <param name="data">Method parameter array (null for parameterless method)</param>
        /// <param name="t">Parameter types, must be in the same order as values in parameter array</param>
        /// <returns>Method's return value if not void</returns>
        public static object InvokeGenericStaticMethod(Type objectType, string methodName, object[] data = null, params Type[] t)
        {
            var method = objectType.GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (method == null)
            {
                throw new Exception(string.Format("Method <{0}> not found in object type <{1}>.", methodName, objectType.Name));
            }
            var generic = method.MakeGenericMethod(t);
            return generic.Invoke(null, data);
        }

        /// <summary>
        /// Invoke generic static method in a class (with a single parameter).
        /// </summary>
        /// <param name="objectType">Object type containing the method to invoke</param>
        /// <param name="methodName">Method name to invoke</param>
        /// <param name="data">Method's parameter</param>
        /// <param name="t">Parameter type</param>
        /// <returns>Method's return value if not void</returns>
        public static object InvokeGenericStaticMethod(Type objectType, string methodName, object data, Type t)
        {
            return InvokeGenericStaticMethod(objectType, methodName, new[] { data }, t);
        }

        /// <summary>
        /// Invoke static generic method in a class (generic).
        /// </summary>
        /// <typeparam name="T">Object type containing the method to invoke</typeparam>
        /// <param name="methodName">Method name to invoke</param>
        /// <param name="data">Method parameter array (null for parameterless method)</param>
        /// <param name="t">Parameter types, must be in the same order as values in parameter array</param>
        /// <returns>Method's return value if not void</returns>
        public static object InvokeGenericStaticMethod<T>(string methodName, object[] data = null, params Type[] t)
        {
            var objectType = typeof(T);
            return InvokeGenericStaticMethod(objectType, methodName, data, t);
        }

        /// <summary>
        /// Invoke static generic method in a class (generic with a single parameter).
        /// </summary>
        /// <typeparam name="T">Object type containing the method to invoke</typeparam>
        /// <param name="methodName">Method name to invoke</param>
        /// <param name="data">Method's parameter</param>
        /// <param name="t">Parameter type</param>
        /// <returns>Method's return value if not void</returns>
        public static object InvokeGenericStaticMethod<T>(string methodName, object data, Type t)
        {
            return InvokeGenericStaticMethod<T>(methodName, new[] { data }, t);
        }

        /// <summary>
        /// Invoke static generic method in a class.
        /// </summary>
        /// <param name="source">Object instance containing the method to invoke</param>
        /// <param name="methodName">Method name to invoke</param>
        /// <param name="data">Method parameter array (null for parameterless method)</param>
        /// <param name="t">Parameter types, must be in the same order as values in parameter array</param>
        /// <returns>Method's return value if not void</returns>
        public static object InvokeGenericMethod(object source, string methodName, object[] data = null, params Type[] t)
        {
            var method = source.GetType().GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var generic = method.MakeGenericMethod(t);
            return generic.Invoke(source, data);
        }

        /// <summary>
        /// Invoke generic instance method in a class (with a single parameter).
        /// </summary>
        /// <param name="source">Object instance containing the method to invoke</param>
        /// <param name="methodName">Method name to invoke</param>
        /// <param name="data">Method's parameter</param>
        /// <param name="t">Parameter type</param>
        /// <returns>Method's return value if not void</returns>
        public static object InvokeGenericMethod(object source, string methodName, object data, Type t)
        {
            return InvokeGenericMethod(source, methodName, new[] { data }, t);
        }

        /// <summary>
        /// Invoke static method in a class.
        /// </summary>
        /// <param name="objectType">Object type containing the method to invoke</param>
        /// <param name="methodName">Method name to invoke</param>
        /// <param name="data">Method parameter array (null for parameterless method)</param>
        /// <returns>Method's return value if not void</returns>
        public static object InvokeStaticMethod(Type objectType, string methodName, object[] data = null)
        {
            var method = objectType.GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return method.Invoke(null, data);
        }

        /// <summary>
        /// Invoke static method in a class (with a single parameter).
        /// </summary>
        /// <param name="objectType">Object type containing the method to invoke</param>
        /// <param name="methodName">Method name to invoke</param>
        /// <param name="data">Method's parameter</param>
        /// <returns>Method's return value if not void</returns>
        public static object InvokeStaticMethod(Type objectType, string methodName, object data)
        {
            return InvokeStaticMethod(objectType, methodName, new[] { data });
        }

        /// <summary>
        /// Invoke static static method in a class (generic).
        /// </summary>
        /// <typeparam name="T">Object type containing the method to invoke</typeparam>
        /// <param name="methodName">Method name to invoke</param>
        /// <param name="data">Method parameter array (null for parameterless method)</param>
        /// <returns>Method's return value if not void</returns>
        public static object InvokeStaticMethod<T>(string methodName, object[] data = null)
        {
            var objectType = typeof(T);
            return InvokeStaticMethod(objectType, methodName, data);
        }

        /// <summary>
        /// Invoke static method in a class (generic with a single parameter).
        /// </summary>
        /// <typeparam name="T">Object type containing the method to invoke</typeparam>
        /// <param name="methodName">Method name to invoke</param>
        /// <param name="data">Method's parameter</param>
        /// <returns>Method's return value if not void</returns>
        public static object InvokeStaticMethod<T>(string methodName, object data)
        {
            return InvokeStaticMethod<T>(methodName, new[] { data });
        }

        /// <summary>
        /// Invoke instance method in a class.
        /// </summary>
        /// <param name="source">Object instance containing the method to invoke</param>
        /// <param name="methodName">Method name to invoke</param>
        /// <param name="data">Method parameter array (null for parameterless method)</param>
        /// <returns>Method's return value if not void</returns>
        public static object InvokeMethod(object source, string methodName, object[] data = null)
        {
            var method = source.GetType().GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return method.Invoke(source, data);
        }

        /// <summary>
        /// Invoke instance method in a class (with a single parameter).
        /// </summary>
        /// <param name="source">Object instance containing the method to invoke</param>
        /// <param name="methodName">Method name to invoke</param>
        /// <param name="data">Method's parameter</param>
        /// <returns>Method's return value if not void</returns>
        public static object InvokeMethod(object source, string methodName, object data)
        {
            return InvokeMethod(source, methodName, new[] { data });
        }

        /// <summary>
        /// Get member attribute of type T.
        /// </summary>
        /// <typeparam name="T">Attribute type to search for</typeparam>
        /// <param name="member">Member to search, can be type, field, property, or method</param>
        /// <param name="inherit">Search inheritance if applicable</param>
        /// <returns>Attribute instance if found, otherwise null</returns>
        public static T GetAttribute<T>(this MemberInfo member, bool inherit = true)
        {
            return (T)member.GetCustomAttributes(typeof(T), inherit).FirstOrDefault();
        }

        /// <summary>
        /// Get all member attributes of type T.
        /// </summary>
        /// <typeparam name="T">Attribute type to search for</typeparam>
        /// <param name="member">Member to search, can be type, field, property, or method</param>
        /// <param name="inherit">Search inheritance if applicable</param>
        /// <returns>Enumeration of all attributes of attribute type</returns>
        public static IEnumerable<T> GetAttributes<T>(this MemberInfo member, bool inherit = true)
        {
            return member.GetCustomAttributes(typeof(T), inherit).Cast<T>();
        }

        /// <summary>
        /// Check if member has an attribute of type T
        /// </summary>
        /// <typeparam name="T">Attribute type to search for</typeparam>
        /// <param name="member">Member to search, can be type, field, property, or method</param>
        /// <returns>True if attribute T exists, otherwise False</returns>
        public static bool HasAttribute<T>(this MemberInfo member)
        {
            return member.GetAttribute<T>() != null;
        }

        /// <summary>
        /// Check if generic type has a generic argument of specified type.
        /// </summary>
        /// <param name="type">Generic type</param>
        /// <param name="argType">Argument type</param>
        /// <returns>True if argument type exists, otherwise False</returns>
        public static bool HasGenericArgument(this Type type, Type argType)
        {
            var args = type.GetGenericArguments();
            return args.Any(att => att == argType) ||
                type.BaseType != null && type.BaseType.HasGenericArgument(argType);
        }

        /// <summary>
        /// Check if generic type has a generic argument of specified type (generic).
        /// </summary>
        /// <typeparam name="T">Argument type</typeparam>
        /// <param name="type">Generic type</param>
        /// <returns>True if argument type exists, otherwise False</returns>
        public static bool HasGenericArgument<T>(this Type type)
        {
            return type.HasGenericArgument(typeof(T));
        }

        /// <summary>
        /// Traverse inheritance hierarchy to find first type with generic arguments and return them.
        /// </summary>
        /// <param name="type">
        /// Type to search, if generic it's arguments will be returned, otherwise it's base class will be
        /// searched recursively
        /// </param>
        /// <returns>Array of generic types, null if none were found</returns>
        public static Type[] GetGenericArgumentsEx(this Type type)
        {
            var args = type.GetGenericArguments();
            return args.Length > 0 ? args : type.BaseType?.GetGenericArgumentsEx();
        }
        #endregion
    }
}