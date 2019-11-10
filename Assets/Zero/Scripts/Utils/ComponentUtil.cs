using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 组件工具
    /// </summary>
    public class ComponentUtil
    {
        /// <summary>
        /// 获取对象上对应的组件，如果没有则添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static T AutoGet<T>(GameObject gameObject) where T : Component
        {
            if(null == gameObject)
            {
                return null;
            }

            T component = gameObject.GetComponent<T>();
            if (null == component)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }


    }
}
