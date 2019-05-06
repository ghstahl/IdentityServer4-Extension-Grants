﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace P7Core.Reflection
{
    public static class TypeExtensions
    {
        public static bool IsPublicClass(this Type type)
        {
            return (type != null
                    && type.GetTypeInfo().IsPublic
                    && type.GetTypeInfo().IsClass
                    && !type.GetTypeInfo().IsAbstract);
        }

        public static IEnumerable<Type> WithCustomAttribute<TAttributeType>(this IEnumerable<Type> master)
        {
            return
                master.Where(type => type.GetTypeInfo().GetCustomAttributes(typeof(TAttributeType), true).Any())
                    .ToList();
        }

        public static bool IsGenericList(this object obj)
        {
            var oType = obj.GetType();
            return (oType.GetTypeInfo().IsGenericType && (oType.GetGenericTypeDefinition() == typeof(List<>)));
        }
        public static IEnumerable<FieldInfo> GetConstants(this Type type)
        {
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly);
        }
        public static IEnumerable<FieldInfo> GetConstants<T>(this Type type)
        {
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var query = from fi in fieldInfos
                where fi.FieldType == typeof(T) && fi.IsLiteral && !fi.IsInitOnly
                select fi;

            return query;
        }
        public static IEnumerable<T> GetConstantsValues<T>(this IEnumerable<FieldInfo> fieldInfos) 
        {
            var query = from fi in fieldInfos
                where fi.FieldType == typeof(T)
                select (T)fi.GetRawConstantValue();

            return query;
        }

        public static IEnumerable<T> GetConstantsValues<T>(this Type type) 
        {
            var fieldInfos = type.GetConstants<T>();
            return fieldInfos.GetConstantsValues<T>();
        }

        public static string AssemblyNameWithoutVersion(this Assembly assembly)
        {
            var fullNameSplit = assembly.FullName.Split(',');
            return fullNameSplit[0];
        }
        public static string AssemblyQualifiedNameWithoutVersion(this Type type)
        {
            var shortAssemblyName = type.GetTypeInfo().Assembly.AssemblyNameWithoutVersion();
            return type.FullName + "," + shortAssemblyName;
        }

        /*
        public static RouteConstraintAttribute GetRouteConstraintAttribute<TAttributeType>(this Type type)
        {
            var attrib = type.GetTypeInfo().GetCustomAttributes(typeof (TAttributeType), true);
            if (attrib.Any())
            {
                return (RouteConstraintAttribute) attrib.First();
            }
            return null;
        }
*/

    }
}