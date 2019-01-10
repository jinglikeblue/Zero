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
        public struct ResVO
        {            
            public string key;
            public UnityEngine.Object[] list;
        }

        [Header("数据引用")]
        public ResVO[] resList;

        /// <summary>
        /// 找到Key对应的资源
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ResVO? Find(string key)
        {
            foreach (var vo in resList)
            {
                if (vo.key == key)
                {                    
                    return vo;
                }
            }
            
            return null;
        }
    }
}