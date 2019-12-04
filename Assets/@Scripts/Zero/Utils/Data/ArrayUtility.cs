using System.Collections.Generic;
using UnityEngine;

namespace ZeroHot
{
    /// <summary>
    /// 数组工具类
    /// </summary>
    public class ArrayUtility
    {
        /// <summary>
        /// 将队列转换成词典
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="collection"></param>
        /// <param name="keyFieldName"></param>
        /// <returns></returns>
        static public Dictionary<TKey, TValue> Array2Table<TKey, TValue>(IEnumerable<TValue> collection, string keyFieldName, string tableName = "")
        {
            if (null == collection)
            {
                return null;
            }

            var enumerator = collection.GetEnumerator();
            Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();

            var type = typeof(TValue);
            var fi = type.GetField(keyFieldName);

            while (enumerator.MoveNext())
            {
                var keyValue = (TKey)fi.GetValue(enumerator.Current);
                if (dic.ContainsKey(keyValue))
                {
                    Debug.LogErrorFormat("构建表[{0}]检测到重复的Key:{1} Value:{2}", tableName, keyFieldName, keyValue);
                }
                else
                {
                    dic.Add(keyValue, enumerator.Current);
                }
            }

            return dic;
        }
    }
}
