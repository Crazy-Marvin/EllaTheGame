using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MonKey.Extensions
{

    public static class ListExt
    {

        public static bool IsEmpty<T>(this List<T> list)
        {
            return (list.Count == 0);
        }

        public static List<T> RemoveReturn<T>(this List<T> list, T toRemove)
        {
            list.Remove(toRemove);
            return list;
        }

        public static List<T> AddReturn<T>(this List<T> list, T toAdd)
        {
            list.Add(toAdd);
            return list;
        }

        public static List<T> AddRangeReturn<T>(this List<T> list, IEnumerable<T> toAdd)
        {
            list.AddRange(toAdd);
            return list;
        }

        public static List<T> AddRangeReturn<T>(this List<T> list, T[] toAdd)
        {
            list.AddRange(toAdd);
            return list;
        }

        public static bool IsEmpty<T>(this IList<T> lst)
        {
            return lst.Count == 0;
        }

        public static T GetRandom<T>(this IList<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }

        public static T GetWeightedRandom<T>(this IList<T> from, IList<float> weights, float totalWeight)
        {
            Debug.Assert(from.Count == weights.Count, "The Random element cannot be retrieved" +
                                                   " because the count of weights and " +
                                                   "elements isn't the same");
            float f = Random.Range(0, totalWeight);
            float cumulative = weights[0];

            for (int i = 0; i < from.Count; i++)
            {
                if (Mathf.Approximately(weights[i], 0))
                    continue;

                if (f > cumulative)
                {
                    cumulative += weights[i];
                }
                else
                {
                    return from[i];
                }
            }
            //shouldn't happen
            return from.Last();
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;

            while (n > 1)
            {
                n--;
                var k = Random.Range(0, n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (var i in ie)
            {
                action(i);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> ie, Action<T, int> action)
        {
            int i = 0;
            foreach (var e in ie)
            {
                action(e, i);
                i++;
            }
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> en, IEnumerable<T> toAppend)
        {
            List<T> appened = en.ToList();
            appened.AddRange(toAppend);
            return appened;
        }

        public static IEnumerable<T> Convert<T, T1>(this IEnumerable<T1> en, Func<T1, T> conversion)
        {
            List<T> newList = new List<T>();

            foreach (var member in en)
            {
                newList.Add(conversion(member));
            }
            return newList;
        }

        public static int NextAvailableID(this IEnumerable<int> ids)
        {
            return Enumerable.Range(0, int.MaxValue).Except(ids).FirstOrDefault();
        }

        public static bool ContainsAny<T>(this IEnumerable<T> source, IEnumerable<T> compare)
        {
            var enumerable = compare as T[] ?? compare.ToArray();
            foreach (var t in source)
            {
                if (enumerable.Contains(t))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a range of int, both extremes included
        /// </summary>
        /// <param name="source"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void AddRange(this IList<int> source,int from,int to)
        {
            for (int i = from; i <= to; i++)
            {
                source.Add(i);
            }
        }
    }
}
