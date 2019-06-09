using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdentityServer4.Contrib.Cosmonaut.Extensions
{
    public static class ComparerExtensions
    {
        static bool CompareLists<T>(this List<T> list1, List<T> list2, Func<T, T, bool> comparer)
        {
            //here we check the count of list elements if they match, it can work also if the list count doesn't meet, to do it just comment out this if statement
            if (list1.Count != list2.Count)
                return false;

            //here we check and find every element from the list1 in the list2
            foreach (var item in list1)
                if (list2.Find(i => comparer(i, item)) == null)
                    return false;

            //here we check and find every element from the list2 in the list1 to make sure they don't have repeated and mismatched elements
            foreach (var item in list2)
                if (list1.Find(i => comparer(i, item)) == null)
                    return false;

            //return true because we didn't find any missing element
            return true;
        }

        static bool CompareLists<T>(this List<T> list1, List<T> list2)
        {
            //here we check the count of list elements if they match, it can work also if the list count doesn't meet, to do it just comment out this if statement
            if (list1.Count != list2.Count)
                return false;

            //here we check and find every element from the list1 in the list2
            foreach (var item in list1)
                if (list2.Find(i => i.Equals(item)) == null)
                    return false;

            //here we check and find every element from the list2 in the list1 to make sure they don't have repeated and mismatched elements
            foreach (var item in list2)
                if (list1.Find(i => i.Equals(item)) == null)
                    return false;

            //return true because we didn't find any missing element
            return true;
        }

        public static bool DeepCompare(this Resource first, Resource second)
        {
            if (ReferenceEquals(first, second)) return true;
            if ((first == null) || (second == null)) return false;

            if (first.Description != second.Description) return false;
            if (first.DisplayName != second.DisplayName) return false;
            if (first.Enabled != second.Enabled) return false;
            if (first.Name != second.Name) return false;
            if (!first.UserClaims.ToList().CompareLists(second.UserClaims.ToList())) return false;
            if (first.Properties.Count != second.Properties.Count) return false;
            var dict3 = first.Properties.Where(entry => second.Properties[entry.Key] != entry.Value)
                 .ToDictionary(entry => entry.Key, entry => entry.Value);
            if (dict3.Any()) return false;

            return true;
        }
        public static bool DeepCompare(this Secret first, Secret second)
        {
            if (ReferenceEquals(first, second)) return true;
            if ((first == null) || (second == null)) return false;

            if (first.Description != second.Description) return false;
            if (first.Expiration != second.Expiration) return false;
            if (first.Type != second.Type) return false;
            if (first.Value != second.Value) return false;
            return true;
        }
        public static bool DeepCompare(this Scope first, Scope second)
        {
            if (ReferenceEquals(first, second)) return true;
            if ((first == null) || (second == null)) return false;

            if (first.Description != second.Description) return false;
            if (first.DisplayName != second.DisplayName) return false;
            if (first.Emphasize != second.Emphasize) return false;
            if (first.Name != second.Name) return false;
            if (first.Required != second.Required) return false;
            if (first.ShowInDiscoveryDocument != second.ShowInDiscoveryDocument) return false;
            if (!first.UserClaims.ToList().CompareLists(second.UserClaims.ToList())) return false;
            return true;
        }
        public static bool DeepCompare(this ApiResource first, ApiResource second)
        {

            if (ReferenceEquals(first, second)) return true;
            if ((first == null) || (second == null)) return false;

            if (!CompareLists(first.ApiSecrets.ToList(), second.ApiSecrets.ToList(), (t1, t2) =>
              {
                  return DeepCompare(t1, t2);
              })) return false;

            if (!CompareLists(first.Scopes.ToList(), second.Scopes.ToList(), (t1, t2) =>
            {
                return DeepCompare(t1, t2);
            })) return false;

            if (!DeepCompare((Resource)first, (Resource)second)) return false;
            return true;
        }

    }
}
