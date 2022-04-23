using System.Collections.Generic;

namespace MonKey.Extensions
{
   public static class DictionaryExt
    {
        public static void AddRange<T, T1>(this Dictionary<T, T1> dic, Dictionary<T, T1> otherDic)
        {
            foreach (var pair in otherDic)
            {
                if(!dic.ContainsKey(pair.Key))
                    dic.Add(pair.Key,pair.Value);
            }
        }

        public static void AddRange<T, T1>(this Dictionary<T, object> dic, Dictionary<T, T1> otherDic)
        {
            foreach (var pair in otherDic)
            {
                if (!dic.ContainsKey(pair.Key))
                    dic.Add(pair.Key, pair.Value);
            }
        }
    }
}
