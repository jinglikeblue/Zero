using System;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 资源引用组件
    /// </summary>
    public class ObjectBindingData : MonoBehaviour
    {
        [Serializable]
        public struct BindingVO
        {            
            public string key;
            public UnityEngine.Object[] list;
        }

        [Header("数据引用")]
        public BindingVO[] resList;

        /// <summary>
        /// 找到Key对应的资源
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public UnityEngine.Object[] Find(string key)
        {
            foreach (var vo in resList)
            {
                if (vo.key == key)
                {                    
                    return vo.list;
                }
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