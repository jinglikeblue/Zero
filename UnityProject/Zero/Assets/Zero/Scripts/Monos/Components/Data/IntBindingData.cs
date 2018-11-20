using System;
using UnityEngine;

namespace Zero
{
    public class IntBindingData : MonoBehaviour
    {
        [Serializable]
        public struct IntVO
        {
            public string key;
            public int[] list;
        }

        [Header("数据引用")]
        public IntVO[] list;

        /// <summary>
        /// 找到Key对应的资源
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IntVO? Find(string key)
        {
            foreach(var vo in list)
            {
                if(vo.key == key)
                {
                    return vo;
                }
            }
            return null;
        }
    }
}