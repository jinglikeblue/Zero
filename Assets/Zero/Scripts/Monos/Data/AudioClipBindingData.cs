using System;
using UnityEngine;

namespace Zero
{
    public class AudioClipBindingData : BaseBinding
    {
        [Serializable]
        public struct BindingVO
        {
            public string key;
            public AudioClip[] list;
        }

        [Header("数据引用")]
        public BindingVO[] list;

        /// <summary>
        /// 找到Key对应的资源
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public AudioClip[] Find(string key)
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

        public static AudioClip[] Find(GameObject go, string key)
        {
            var data = go.GetComponent<AudioClipBindingData>();
            if (null == data)
            {
                return null;
            }

            return data.Find(key);
        }
    }
}