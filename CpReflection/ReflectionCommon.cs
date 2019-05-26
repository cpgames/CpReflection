using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace cpGames.core.CpReflection
{
    public static class ReflectionCommon
    {
        #region Methods
        public static Type GetElementType(this Type type)
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
            return GetElementType(type.BaseType);
        }

        public static bool IsStruct(this Type type)
        {
            return type.IsValueType && !type.IsPrimitive && !type.IsEnum;
        }
        
        public static IEnumerable<Type> FindAllDerivedTypes(this Type type, Assembly assembly = null, bool includeSelf = false, bool includeAbstract = false)
        {
            if (assembly == null)
                assembly = Assembly.GetAssembly(type);
            return assembly
                .GetTypes()
                .Where(t =>
                    (includeSelf || t != type) &&
                    (includeAbstract || !t.IsAbstract) &&
                    type.IsAssignableFrom(t));
        }
        

        public static IEnumerable<Type> FindAllDerivedTypes<T>(Assembly assembly = null, bool includeSelf = false, bool includeAbstract = false)
        {
            return FindAllDerivedTypes(typeof(T), assembly, includeSelf, includeAbstract);
        }

        public static bool IsTypeOrDerived(this Type derivedType, Type baseType)
        {
            return baseType == derivedType ||
                derivedType.IsSubclassOf(baseType) ||
                baseType.IsAssignableFrom(derivedType);
        }

        public static bool IsTypeOrDerived(object baseObj, object derivedObj)
        {
            return derivedObj.GetType().IsTypeOrDerived(baseObj.GetType());
        }

        public static bool IsTypeOrDerived(Type baseType, object derivedObj)
        {
            return derivedObj.GetType().IsTypeOrDerived(baseType);
        }

        public static object InvokeGeneric<T>(string methodName, Type t, object[] data)
        {
            var method = typeof(T).GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var generic = method.MakeGenericMethod(t);
            return generic.Invoke(null, data);
        }

        public static object InvokeGeneric<T>(string methodName, Type t, object data)
        {
            var method = typeof(T).GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var generic = method.MakeGenericMethod(t);
            return generic.Invoke(null, new[] { data });
        }

        public static object InvokeGeneric<T>(string methodName, Type t)
        {
            var method = typeof(T).GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var generic = method.MakeGenericMethod(t);
            return generic.Invoke(null, null);
        }

        public static object InvokeGeneric<T>(object source, string methodName, Type t)
        {
            var method = typeof(T).GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var generic = method.MakeGenericMethod(t);
            return generic.Invoke(source, null);
        }

        public static object InvokeMethod(object source, string methodName, object[] data)
        {
            var method = source.GetType().GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return method.Invoke(source, data);
        }

        public static IEnumerable<FieldInfo> GetFields(this Type type)
        {
            var fields =
                type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return fields;
        }

        public static T Factory<T>(Func<Type, object> factoryMethod, Type type) where T : class
        {
            T res = null;

            if (factoryMethod != null)
            {
                res = (T)factoryMethod(type);
            }
            if (res == null)
            {
                var ctor = type.GetConstructor(Type.EmptyTypes);
                res = (T)ctor.Invoke(null);
            }
            return res;
        }

        public static T GetAttribute<T>(this FieldInfo field, bool inherit = true)
        {
            return (T)field.GetCustomAttributes(typeof(T), inherit).FirstOrDefault();
        }

        public static List<T> GetAttributes<T>(this FieldInfo field, bool inherit = true)
        {
            return field.GetCustomAttributes(typeof(T), inherit).Cast<T>().ToList();
        }

        public static T GetAttribute<T>(this Type type, bool inherit = true)
        {
            return (T)type.GetCustomAttributes(typeof(T), true).FirstOrDefault();
        }

        public static T GetAttribute<T>(this MethodInfo method, bool inherit = true)
        {
            return (T)method.GetCustomAttributes(typeof(T), inherit).FirstOrDefault();
        }

        public static T GetAttribute<T>(this PropertyInfo property, bool inherit = true)
        {
            return (T)property.GetCustomAttributes(typeof(T), inherit).FirstOrDefault();
        }

        public static List<T> GetAttributes<T>(this Type type, bool inherit = true)
        {
            return type.GetCustomAttributes(typeof(T), inherit).Cast<T>().ToList();
        }

        public static List<T> GetAttributes<T>(this MethodInfo method, bool inherit = true)
        {
            return method.GetCustomAttributes(typeof(T), inherit).Cast<T>().ToList();
        }

        public static bool HasAttribute<T>(this FieldInfo field)
        {
            return field.GetAttribute<T>() != null;
        }

        public static bool HasAttribute<T>(this Type type)
        {
            return type.GetAttribute<T>() != null;
        }

        public static bool HasAttribute<T>(this MethodInfo method)
        {
            return method.GetAttribute<T>() != null;
        }

        public static bool HasAttribute<T>(this PropertyInfo property)
        {
            return property.GetAttribute<T>() != null;
        }

        public static bool HasGenericArgument(this Type type, Type argType)
        {
            var args = type.GetGenericArguments();
            return args.Any(att => att == argType) ||
                type.BaseType != null && type.BaseType.HasGenericArgument(argType);
        }

        public static bool HasGenericArgument<T>(this Type type)
        {
            return type.HasGenericArgument(typeof(T));
        }

        public static Type[] GetBaseGenericArguments(this Type type)
        {
            var args = type.GetGenericArguments();
            if (args.Length > 0)
            {
                return args;
            }
            return type.BaseType?.GetBaseGenericArguments();
        }
        #endregion
    }
}