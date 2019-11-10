using System;
using UnityEngine;

namespace Zero
{
    public class IntBindingData : MonoBehaviour
    {
        [Serializable]
        public struct BindingVO
        {
            public string key;
            public int[] list;
        }

        [Header("数据引用")]
        public BindingVO[] list;

        /// <summary>
        /// 找到Key对应的资源
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int[] Find(string key)
        {
            foreach(var vo in list)
            {
                if(vo.key == key)
                {
                    return vo.list;
                }
            }
            return null;
        }

        public static int[] Find(GameObject go, string key)
        {
            var data = go.GetComponent<IntBindingData>();
            if (null == data)
            {
                return null;
            }

            return data.Find(key);
        }
    }
}