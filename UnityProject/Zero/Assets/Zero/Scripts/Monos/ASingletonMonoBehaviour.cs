using UnityEngine;

namespace Zero
{
    /// <summary>
    /// MonoBehaviour的单例基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ASingletonMonoBehaviour<T> : MonoBehaviour where T : Component
    {
        private static T _ins;

        /// <summary>
        /// 获得单例
        /// </summary>
        public static T Ins
        {
            get
            {
                if (null == _ins)
                {                    
                    _ins = GameObject.FindObjectOfType<T>();

                    if (null == _ins)
                    {
                        var go = new GameObject();
                        go.name = typeof(T).Name;
                        _ins = go.AddComponent<T>();                        
                    }

                    DontDestroyOnLoad(_ins.gameObject);
                }
                return _ins;
            }
        }

        /// <summary>
        /// 销毁当前单例
        /// </summary>
        public static void DestroyIns()
        {
            if (null != _ins)
            {                
                GameObject.Destroy(_ins.gameObject);
                _ins = null;
            }
        }
    }
}