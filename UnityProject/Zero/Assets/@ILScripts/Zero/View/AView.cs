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
        /// 通过Prefab创建AView对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab">Prefab对象</param>
        /// <param name="parentView">作为父对象的AView</param>
        /// <param name="parentTransform">父Transform容器</param>
        /// <param name="data">携带的参数</param>
        /// <returns></returns>
        public static T Create<T>(GameObject prefab, AView parentView, Transform parentTransform, object data = null) where T : AView
        {
            if(null == prefab)
            {
                return null;
            }
            var entry = new ViewEntry(null, prefab.name, typeof(T));
            AView view = AViewMgr.CreateViewFromPrefab(prefab, parentTransform, entry);
            //parentView.AddChild(view);
            if (data != null)
            {
                view.SetData(data);
            }
            return view as T;
        }

        /// <summary>
        /// 异步创建AView对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewName">Prefab名称</param>
        /// <param name="abName">Prefab所在资源AB包</param>
        /// <param name="parentView">作为父对象的AView</param>
        /// <param name="parentTransform">父Transform容器</param>
        /// <param name="data">携带的参数</param>
        /// <param name="onCreated">创建完成时调用</param>
        /// <param name="onProgress">创建进度</param>
        public static void CreateAsync<T>(string viewName, string abName, AView parentView, Transform parentTransform, object data = null, Action<T> onCreated = null, Action<float> onProgress = null) where T : AView
        {
            ResMgr.Ins.LoadAsync(abName, viewName,
            (UnityEngine.Object asset) =>
            {
                GameObject prefab = asset as GameObject;
                T view = Create<T>(prefab, parentView, parentTransform, data);
                onCreated?.Invoke(view);
            },
            onProgress);
        }

        /// <summary>
        /// 关联的父级对象
        /// </summary>
        protected AView parent { get; private set; }

        /// <summary>
        /// 销毁委托事件
        /// </summary>
        public event Action<AView> onDestroyHandler;

        /// <summary>
        /// Unity中的GameObject对象
        /// </summary>
        public GameObject GameObject { get; private set; }

        /// <summary>
        /// 是否销毁了
        /// </summary>
        public bool IsDestroyed
        {
            get { return GameObject == null ? true : false; }
        }        

        /// <summary>
        /// 对象名称
        /// </summary>
        public string Name
        {
            get { return GameObject.name; }
            set { GameObject.name = value; }
        }
        
        /// <summary>
        /// 对象对应的视图的信息
        /// </summary>
        public ViewEntry ViewEntry { get; internal set; }

        /// <summary>
        /// 子视图对象的列表
        /// </summary>
        //List<AView> _childViewList = new List<AView>();                    

        /// <summary>
        /// 挂载到GameObject上的脚本
        /// </summary>
        ZeroView _z;

        internal void SetGameObject(GameObject gameObject)
        {
            this.GameObject = gameObject;

            _z = ComponentUtil.AutoGet<ZeroView>(this.GameObject);
            _z.onEnable += OnGameObjectEnable;
            _z.onDisable += OnGameObjectDisable;
            _z.onDestroy += OnGameObjectDestroy;

            OnInit();
            if (this.GameObject.activeInHierarchy)
            {
                OnEnable();
            }
        }

        internal void SetData(object data)
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
            AViewMgr.DestroyView(this);
            _z = null;
            GameObject = null;
        }

        /// <summary>
        /// 设置激活
        /// </summary>
        /// <param name="isActive"></param>
        public void SetActive(bool isActive)
        {
            if(isActive)
            {
                if(false == GameObject.activeInHierarchy)
                {
                    GameObject.SetActive(true);
                    //WhenEnable();
                }
            }
            else
            {
                if(GameObject.activeInHierarchy)
                {
                    //WhenDisable();
                    GameObject.SetActive(false);                    
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
            return GameObject.GetComponent<T>();
        }

        /// <summary>
        /// 得到组件(如果没有则自动添加)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AudoGetComponent<T>() where T :Component
        {
            return ComponentUtil.AutoGet<T>(GameObject);
        }

        /// <summary>
        /// 得到子对象上的组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="childName"></param>
        /// <returns></returns>
        public T GetChildComponent<T>(string childName)
        {
            var child = GameObject.transform.Find(childName);
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
            return GameObject.transform.Find(childName);
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
            if (null == item)
            {
                return null;
            }
            return item.Value.list;
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
            if (null == item)
            {
                return null;
            }
            return item.Value.list;
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

            if (null == item)
            {
                return null;
            }
            return item.Value.list;
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
            if (null == item)
            {
                return null;
            }
            return item.Value.list;
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
            if (null == item)
            {
                return null;
            }
            return item.Value.list;
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
            viewChild.SetGameObject(childGameObject);
            if (data != null)
            {
                viewChild.SetData(data);
            }

            //AddChild(viewChild);

            return viewChild;
        }

        /// <summary>
        /// 从父对象移除
        /// </summary>
        //void RemoveFromParent()
        //{
        //    if (null != parent)
        //    {
        //        parent.RemoveChild(this);                
        //    }
        //}

        /// <summary>
        /// 添加子视图
        /// </summary>
        /// <param name="child"></param>
        //private void AddChild(AView child)
        //{            
        //    if(child.parent != this)
        //    {
        //        child.RemoveFromParent();
        //        child.parent = this;
        //        _childViewList.Add(child);
        //    }
        //}

        /// <summary>
        /// 移除子视图
        /// </summary>
        /// <param name="child"></param>
        //public void RemoveChild(AView child)
        //{
        //    if (child.parent == this)
        //    {
        //        child.parent = null;
        //        _childViewList.Remove(child);
        //    }
        //}

        /// <summary>
        /// 销毁对象
        /// </summary>
        public void Destroy()
        {
            if (IsDestroyed)
            {
                return;
            }

            GameObject.Destroy(GameObject);


            //WhenDisable();
            //RemoveFromParent();

            //WhenDestroy();
        }

        //internal void WhenEnable()
        //{
        //    if(false == gameObject.activeSelf)
        //    {
        //        return;
        //    }


        //    OnEnable();
        //    foreach (var childView in _childViewList)
        //    {
        //        childView.WhenEnable();
        //    }            
        //}

        //internal void WhenDisable()
        //{
        //    if (false == gameObject.activeSelf)
        //    {
        //        return;
        //    }

        //    OnDisable();
        //    foreach (var childView in _childViewList)
        //    {                
        //        childView.WhenDisable();                
        //    }            
        //}

        //internal void WhenDestroy()
        //{
        //    OnDestroy();
        //    if (onDestroyHandler != null)
        //    {
        //        onDestroyHandler.Invoke(this);
        //    }

        //    foreach (var childView in _childViewList)
        //    {
        //        childView.WhenDestroy();
        //    }
        //}

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
