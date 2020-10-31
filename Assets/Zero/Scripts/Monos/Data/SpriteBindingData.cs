using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero
{
    public class SpriteBindingData : BaseBinding
    {
        [Serializable]
        public struct BindingVO
        {
            public string key;
            public Sprite[] list;
        }

        [Header("数据引用")]
        public BindingVO[] list;

        /// <summary>
        /// 找到Key对应的资源
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Sprite[] Find(string key)
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

        public static Sprite[] Find(GameObject go, string key)
        {            
            var data = go.GetComponent<SpriteBindingData>();
            if (null == data)
            {
                return null;
            }

            return data.Find(key);
        }
    }
}