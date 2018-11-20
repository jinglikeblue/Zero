using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero
{
    public class FloatBindingData : MonoBehaviour
    {
        [Serializable]
        public struct FloatVO
        {
            public string key;
            public float[] list;
        }

        [Header("数据引用")]
        public FloatVO[] list;

        /// <summary>
        /// 找到Key对应的资源
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public FloatVO? Find(string key)
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