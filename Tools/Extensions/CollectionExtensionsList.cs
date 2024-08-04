using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Common.Extensions
{
    public static class CollectionExtensionsList
    {
        public static T[,] Get2DArrayFromLists<T>(this List<List<T>> lists)
        {
            var output = new T[lists.Count, lists.Max(i => i.Count)];
            for (int i = 0; i < lists.Count; ++i)
                for (int j = 0; j < lists[i].Count; ++j)
                    output[i, j] = lists[i][j];

            return output;
        }

        public static TOut[,] Select2DArrayFromLists<TIn, TOut>(this List<List<TIn>> lists, Func<TIn, TOut> func)
        {
            var output = new TOut[lists.Count, lists.Max(i => i.Count)];
            for (int i = 0; i < lists.Count; ++i)
               for (int j = 0; j < lists[i].Count; ++j)
                    output[i, j] = func(lists[i][j]);

            return output;
        }
        
        public static string ToPrettyString<TK, TV>(this IDictionary<TK, TV> dictionary,
            Func<TK, string> keyToString,
            Func<TV, string> valueToString)
        {
            if (keyToString == null)
                throw new Exception($"'{nameof(keyToString)}' is cannot be null!");
            
            if (valueToString == null)
                throw new Exception($"'{nameof(valueToString)}' is cannot be null!");
            
            return "{" + string.Join(",", dictionary.Select(kv => keyToString(kv.Key) + "=" + valueToString(kv.Value)).ToArray()) + "}";
        }

        public static string ToPrettyString<TK, TV>(this IDictionary<TK, TV> dictionary)
        {
            return dictionary.ToPrettyString(
                keyToString: k => k.ToString(),
                valueToString: v => v.ToString()
            );
        }

        public static void AddAll<TK, TV>(this IDictionary<TK, TV> dictionary, IDictionary<TK, TV> sourceDictionary)
        {
            if (dictionary == null)
                throw new Exception($"'{nameof(dictionary)}' is cannot be null!");
            
            if (sourceDictionary == null)
                throw new Exception($"'{nameof(sourceDictionary)}' is cannot be null!");
            
            foreach (var keyValuePair in sourceDictionary)
                dictionary.Add(keyValuePair);
        }

        public static T Random<T>(this IReadOnlyList<T> list)
        {
            if (!list.Any())
                throw new Exception("Passed list is empty!");
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
    }
}