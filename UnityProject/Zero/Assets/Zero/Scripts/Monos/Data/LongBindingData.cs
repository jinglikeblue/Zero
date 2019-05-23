using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zero
{
    public class LongBindingData : MonoBehaviour
    {
        [Serializable]
        public struct BindingVO
        {
            public string key;
            public long[] list;
        }

        [Header("数据引用")]
        public BindingVO[] list;

        /// <summary>
        /// 找到Key对应的资源
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long[] Find(string key)
        {
            foreach (var vo in list)
            {
                if (vo.key == key)
                {
                    return vo.list;
                }
            }
            return null;
        }

        public static long[] Find(GameObject go, string key)
        {
            var data = go.GetComponent<LongBindingData>();
            if (null == data)
            {
                return null;
            }

            return data.Find(key);
        }
    }
}