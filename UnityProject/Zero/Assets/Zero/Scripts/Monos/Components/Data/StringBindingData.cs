using System;
using UnityEngine;

namespace Zero
{
    public class StringBindingData : MonoBehaviour
    {
        [Serializable]
        public struct StringVO
        {
            public string key;
            public string[] list;
        }

        [Header("数据引用")]
        public StringVO[] list;

        /// <summary>
        /// 找到Key对应的资源
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public StringVO? Find(string key)
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