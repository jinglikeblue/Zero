using System;
using UnityEngine;

namespace Zero
{
    public class GameObjectBindingData : MonoBehaviour
    {
        [Serializable]
        public struct BindingVO
        {
            public string key;
            public GameObject[] list;
        }

        [Header("数据引用")]
        public BindingVO[] list;

        /// <summary>
        /// 找到Key对应的资源
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public GameObject[] Find(string key)
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

        public static GameObject[] Find(GameObject go, string key)
        {
            var data = go.GetComponent<GameObjectBindingData>();
            if (null == data)
            {
                return null;
            }

            return data.Find(key);
        }
    }
}