using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Zero;

namespace IL.Zero
{
    /// <summary>
    /// 视图对象
    /// </summary>
    public abstract class AView
    {
        /// <summary>
        /// 销毁委托事件
        /// </summary>
        public event Action<AView> onDestroyHandler;

        /// <summary>
        /// Unity中的GameObject对象
        /// </summary>
        public GameObject gameObject { get; private set; }        

        /// <summary>
        /// 是否销毁了
        /// </summary>
        public bool IsDestroyed
        {
            get { return gameObject == null ? true : false; }
        }        

        /// <summary>
        /// 对象名称
        /// </summary>
        public string Name
        {
            get { return gameObject.name; }
            set { gameObject.name = value; }
        }                      

        /// <summary>
        /// 挂载到GameObject上的脚本
        /// </summary>
        ZeroView _z;

        /// <summary>
        /// 更新事件
        /// </summary>
        public event Action OnUpdate
        {
            add
            {
                ILBridge.Ins.onUpdate += value;
            }
            remove
            {
                ILBridge.Ins.onUpdate -= value;
            }
        }

        internal void SetGameObject(GameObject gameObject, object data = null)
        {
            this.gameObject = gameObject;

            _z = ComponentUtil.AutoGet<ZeroView>(this.gameObject);
            _z.onEnable += OnGameObjectEnable;
            _z.onDisable += OnGameObjectDisable;
            _z.onDestroy += OnGameObjectDestroy;

            OnInit();

            if(data != null)
            {
                SetData(data);
            }

            if (this.gameObject.activeInHierarchy)
            {
                OnEnable();
            }
        }

        void SetData(object data)
        {
            if (null == data)
            {
                return;
            }
            OnData(data);
        }

        private void OnGameObjectEnable()
        {
            OnEnable();
        }

        private void OnGameObjectDisable()
        {
            OnDisable();
        }

        private void OnGameObjectDestroy()
        {
            _z = null;
            gameObject = null;
            OnDestroy();
            onDestroyHandler?.Invoke(this);
        }

        /// <summary>
        /// 设置激活
        /// </summary>
        /// <param name="isActive"></param>
        public void SetActive(bool isActive)
        {
            if(isActive)
            {
                if(false == gameObject.activeInHierarchy)
                {
                    gameObject.SetActive(true);
                    //WhenEnable();
                }
            }
            else
            {
                if(gameObject.activeInHierarchy)
                {
                    //WhenDisable();
                    gameObject.SetActive(false);                    
                }
            }
        }

        /// <summary>
        /// 得到组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : Component
        {
            return gameObject.GetComponent<T>();
        }

        /// <summary>
        /// 得到组件(如果没有则自动添加)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AudoGetComponent<T>() where T :Component
        {
            return ComponentUtil.AutoGet<T>(gameObject);
        }

        /// <summary>
        /// 得到子对象上的组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="childName"></param>
        /// <returns></returns>
        public T GetChildComponent<T>(string childName)
        {
            var child = gameObject.transform.Find(childName);
            if (null == child)
            {
                return default(T);
            }
            return child.GetComponent<T>();
        }

        /// <summary>
        /// 得到子对象
        /// </summary>
        /// <param name="childName"></param>
        /// <returns></returns>
        public Transform GetChild(string childName)
        {
            return gameObject.transform.Find(childName);
        }    
        
        /// <summary>
        /// 得到子对象
        /// </summary>
        /// <param name="childName"></param>
        /// <returns></returns>
        public GameObject GetChildGameObject(string childName)
        {
            var child = GetChild(childName);
            if(null != child)
            {
                return child.gameObject;
            }
            return null;
        }


        #region 获取绑定的数据

        /// <summary>
        /// 获取绑定的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public UnityEngine.Object[] GetBindingObject(string key)
        {
            var com = GetComponent<ObjectBindingData>();
            if(null == com)
            {
                return null;
            }
            var item = com.Find(key);
            return item;
        }

        /// <summary>
        /// 获取绑定的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public double[] GetBindingDouble(string key)
        {
            var com = GetComponent<DoubleBindingData>();
            if (null == com)
            {
                return null;
            }

            var item = com.Find(key);
            return item;
        }

        /// <summary>
        /// 获取绑定的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public float[] GetBindingFloat(string key)
        {
            var com = GetComponent<FloatBindingData>();
            if (null == com)
            {
                return null;
            }
            var item = com.Find(key);
            return item;
        }

        /// <summary>
        /// 获取绑定的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int[] GetBindingInt(string key)
        {
            var com = GetComponent<IntBindingData>();
            if (null == com)
            {
                return null;
            }
            var item = com.Find(key);
            return item;
        }

        /// <summary>
        /// 获取绑定的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public long[] GetBindingLong(string key)
        {
            var com = GetComponent<LongBindingData>();
            if (null == com)
            {
                return null;
            }
            var item = com.Find(key);
            return item;
        }

        /// <summary>
        /// 获取绑定的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string[] GetBindingString(string key)
        {
            var com = GetComponent<StringBindingData>();
            if (null == com)
            {
                return null;
            }
            var item = com.Find(key);
            return item;
        }

        #endregion



        /// <summary>
        /// 得到子视图对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="childName"></param>
        /// <returns></returns>
        public T CreateViewChlid<T>(string childName, object data = null) where T:AView
        {          
            var childGameObject = GetChildGameObject(childName);
            return CreateViewChlid<T>(childGameObject, data);
        }

        public T CreateViewChlid<T>(GameObject childGameObject, object data = null) where T : AView
        {
            if (null == childGameObject)
            {
                return default(T);
            }

            T viewChild = Activator.CreateInstance(typeof(T)) as T;
            viewChild.SetGameObject(childGameObject, data);            
            return viewChild;
        }

        /// <summary>
        /// 销毁对象
        /// </summary>
        public void Destroy()
        {
            if (IsDestroyed)
            {
                return;
            }

            GameObject.Destroy(gameObject);
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return _z?.StartCoroutine(routine);
        }

        public void StopAllCoroutines()
        {
            _z?.StopAllCoroutines();
        }

        public void StopCoroutine(IEnumerator routine)
        {
            _z?.StopCoroutine(routine);
        }

        public void StopCoroutine(Coroutine routine)
        {
            _z?.StopCoroutine(routine);
        }


        #region 子类按需求重写实现的方法
        /// <summary>
        /// 初始化方法
        /// </summary>
        protected virtual void OnInit()
        {

        }
        


        /// <summary>
        /// 当显示对象被实例化后该方法被调用。在该方法中可以预备视图所需要的数据。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        protected virtual void OnData(object data)
        {

        }

        /// <summary>
        /// 激活时触发
        /// </summary>
        protected virtual void OnEnable()
        {

        }

        /// <summary>
        /// 进入非激活状态时触发
        /// </summary>
        protected virtual void OnDisable()
        {

        }

        /// <summary>
        /// 当显示对象被销毁时调用。在该方法中进行内存回收工作或其它。
        /// </summary>
        protected virtual void OnDestroy()
        {

        }
        #endregion
    }
}
