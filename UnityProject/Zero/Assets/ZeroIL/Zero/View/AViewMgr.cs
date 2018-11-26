using System;
using System.Collections.Generic;
using UnityEngine;
using Zero;

namespace ZeroIL.Zero
{
    public abstract class AViewMgr
    {        
        static protected Dictionary<string, string> _view2ABNameDic = new Dictionary<string, string>();
        static protected Dictionary<string, Type> _view2TypeDic = new Dictionary<string, Type>();
        static protected Dictionary<Type, string> _type2NameDic = new Dictionary<Type, string>();

        /// <summary>
        /// 注册一个界面
        /// </summary>
        /// <param name="viewName">Prefab的名称</param>
        /// <param name="abName">Prefab所在AssetBundle的名称</param>
        /// <param name="type">Prefab的Type</param>
        static public void Regist(string viewName, string abName, Type type)
        {
            _view2TypeDic[viewName] = type;
            _type2NameDic[type] = viewName;
            _view2ABNameDic[viewName] = abName;
        }

        /// <summary>
        /// 注册一个界面，使用这种方式需要确保界面type的Name就是界面的名称
        /// </summary>
        /// <param name="type">界面的Type</param>
        /// <param name="abName"></param>
        static public void Regist(Type type, string abName)
        {
            Regist(type.Name, abName, type);
        }

        /// <summary>
        /// 创建一个视图对象
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="abName"></param>
        /// <param name="type"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        static public AView CreateView(string viewName, string abName, Type type, Transform parent)
        {
            GameObject prefab = ResMgr.Ins.Load<GameObject>(abName, viewName);
            AView view = CreateViewFromPrefab(prefab, parent, viewName, type);
            return view;
        }

        /// <summary>
        /// 通过Prefab创建视图
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <param name="viewName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        static public AView CreateViewFromPrefab(GameObject prefab, Transform parent, string viewName, Type type)
        {
            GameObject go = GameObject.Instantiate(prefab, parent);
            go.name = viewName;

            AView view = Activator.CreateInstance(type) as AView;
            view.SetGameObject(go);
            view.ViewName = viewName;
            return view;
        }

        /// <summary>
        /// 销毁视图对象，同时会回收对应的资源（包含依赖）
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="isGC">是否执行垃圾回收</param>
        static public void DestroyView(AView view)
        {
            if (_view2ABNameDic.ContainsKey(view.ViewName))
            {
                string abName = _view2ABNameDic[view.ViewName];
                ResMgr.Ins.Unload(abName, false, true);
            }
            GameObject.Destroy(view.gameObject);
        }

        protected Transform _root;

        /// <summary>
        /// 是否已经初始化了
        /// </summary>
        public bool IsInited
        {
            get
            {
                return _root == null ? false : true;
            }
        }

        public virtual void Init(Transform root)
        {
            _root = root;
        }

        protected AView CreateView(string viewName)
        {            
            string abName = _view2ABNameDic[viewName];
            Type type = _view2TypeDic[viewName];

            return CreateView(viewName, abName, type, _root);
        }

        protected AView CreateView(Type type)
        {
            string viewName = _type2NameDic[type];
            string abName = _view2ABNameDic[viewName];

            return CreateView(viewName, abName, type, _root);
        }

        protected void CreateViewAsync(string viewName, Action<AView> onCreated, Action<float> onProgress)
        {
            string abName = _view2ABNameDic[viewName];
            Type type = _view2TypeDic[viewName];

            CreateViewAsync(viewName, abName, type, onCreated, onProgress);
            //CoroutineBridge.Ins.StartCoroutine(CreateViewAsync(viewName, abName, type, onCreated, onProgress));            
        }

        protected void CreateViewAsync(Type type, Action<AView> onCreated, Action<float> onProgress)
        {
            string viewName = _type2NameDic[type];
            string abName = _view2ABNameDic[viewName];

            CreateViewAsync(viewName, abName, type, onCreated, onProgress);
            //CoroutineBridge.Ins.StartCoroutine(CreateViewAsync(viewName, abName, type, onCreated, onProgress));
        }

        void CreateViewAsync(string viewName, string abName, Type type, Action<AView> onCreated, Action<float> onProgress)
        {            
            ResMgr.Ins.LoadAsync(abName, viewName, 
                (UnityEngine.Object asset)=> 
                {
                    GameObject prefab = asset as GameObject;
                    AView view = CreateViewFromPrefab(prefab, _root, viewName, type);
                    onCreated(view);
                }, 
                onProgress);
        }

        //IEnumerator CreateViewAsync(string viewName, string abName, Type type, Action<AView> onCreated, Action<float> onProgress)
        //{            
        //    AssetBundle ab = AssetBundleMgr.Ins.LoadAssetBundle(abName);
        //    AssetBundleRequest abr = ab.LoadAssetAsync<GameObject>(viewName);   
        //    while(abr.isDone)
        //    {
        //        if (onProgress != null)
        //        {
        //            onProgress.Invoke(abr.progress);
        //        }
        //        yield return new WaitForEndOfFrame();                
        //    }
        //    //加载完成
        //    if (onProgress != null)
        //    {
        //        onProgress(1f);
        //    }
        //    GameObject prefab = abr.asset as GameObject;
        //    AView view = CreateViewFromPrefab(prefab, _root, viewName, abName, type);
        //    onCreated(view);
        //}
    }
}
