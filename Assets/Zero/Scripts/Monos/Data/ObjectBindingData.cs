using System;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 资源引用组件
    /// </summary>
    public class ObjectBindingData : BaseBinding
    {
        [Serializable]
        public struct BindingVO
        {
            public string key;
            public UnityEngine.Object[] list;
        }

        [Header("数据引用")]
        public BindingVO[] list;

        /// <summary>
        /// 找到Key对应的资源
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public UnityEngine.Object[] Find(string key)
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

        /// <summary>
        /// 获取Key中需要的资源
        /// </summary>
        /// <param name="key"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public UnityEngine.Object Get(string key, int idx)
        {
            var datas = Find(key);
            if (null != datas && datas.Length > idx)
            {
                return datas[idx];
            }
            return null;
        }

        public static UnityEngine.Object[] Find(GameObject go, string key)
        {
            var data = go.GetComponent<ObjectBindingData>();
            if (null == data)
            {
                return null;
            }

            return data.Find(key);
        }
    }
}