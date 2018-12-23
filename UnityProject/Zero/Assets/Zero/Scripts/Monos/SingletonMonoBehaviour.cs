using UnityEngine;

namespace Zero
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T: Component
    {
        private static T _ins;

        /// <summary>
        /// 获得单例
        /// </summary>
        public static T Ins
        {
            get
            {
                if (_ins == null)
                {                    
                    string name = typeof(T).Name;
                    GameObject go = GameObject.Find(name);
                    if (null == go)
                    {
                        go = new GameObject();
                        go.name = name;
                        go.AddComponent<T>();
                    }

                    DontDestroyOnLoad(go);
                    _ins = go.GetComponent<T>();
                }
                return _ins;
            }

            protected set
            {
                if (null == _ins)
                {
                    _ins = value;
                    DontDestroyOnLoad(_ins.gameObject);
                }
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