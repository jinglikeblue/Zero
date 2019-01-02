using System;
using System.Collections.Generic;
using UnityEngine;
using Zero;

namespace IL.Zero
{
    public class ViewEntry
    {
        /// <summary>
        /// AB包名字
        /// </summary>
        public string abName;

        /// <summary>
        /// 视图名字
        /// </summary>
        public string viewName;

        /// <summary>
        /// 类型
        /// </summary>
        public Type type;

        public ViewEntry(string abName, string viewName, Type type)
        {
            this.abName = abName;
            this.viewName = viewName;
            this.type = type;
        }
    }

    public abstract class AViewMgr
    {
        static protected Dictionary<string, Dictionary<string, ViewEntry>> _ab2view2EntryDic = new Dictionary<string, Dictionary<string, ViewEntry>>();
        static protected Dictionary<Type, ViewEntry> _type2EntryDic = new Dictionary<Type, ViewEntry>();

        /// <summary>
        /// 注册一个界面
        /// </summary>
        /// <param name="viewName">Prefab的名称</param>
        /// <param name="abName">Prefab所在AssetBundle的名称</param>
        /// <param name="type">Prefab的Type</param>
        static public void Regist(string viewName, string abName, Type type)
        {
            ViewEntry entry = new ViewEntry(abName, viewName, type);
            if(false == _ab2view2EntryDic.ContainsKey(abName))
            {
                _ab2view2EntryDic[abName] = new Dictionary<string, ViewEntry>();
            }

            _ab2view2EntryDic[abName][viewName] = entry;
            _type2EntryDic[type] = entry;
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
        static public AView CreateView(ViewEntry entry, Transform parent)
        {
            if (null == entry)
            {
                return null;
            }
            GameObject prefab = ResMgr.Ins.Load<GameObject>(entry.abName, entry.viewName);
            AView view = CreateViewFromPrefab(prefab, parent, entry);
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
        static public AView CreateViewFromPrefab(GameObject prefab, Transform parent, ViewEntry entry)
        {
            GameObject go = GameObject.Instantiate(prefab, parent);
            go.name = prefab.name;

            AView view = Activator.CreateInstance(entry.type) as AView;
            view.SetGameObject(go);
            view.ViewEntry = entry;
            return view;
        }

        /// <summary>
        /// 销毁视图对象，同时会回收对应的资源（包含依赖）
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="isGC">是否执行垃圾回收</param>
        static public void DestroyView(AView view)
        {
            if (null != view.ViewEntry && null != view.ViewEntry.abName)
            {
                if (_ab2view2EntryDic.ContainsKey(view.ViewEntry.abName))
                {
                    ResMgr.Ins.Unload(view.ViewEntry.abName, false, true);
                }
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

        protected AView CreateView(string abName, string viewName)
        {            
            ViewEntry entry = GetEntry(abName,viewName);
            return CreateView(entry, _root);
        }

        protected AView CreateView(Type type)
        {
            ViewEntry entry = GetEntry(type);
            if (null == entry)
            {
                Log.E("没有注册的视图：{0}", type.FullName);
            }           
            return CreateView(entry, _root);
        }

        protected void CreateViewAsync(string abName, string viewName, Action<AView> onCreated, Action<float> onProgress)
        {
            ViewEntry entry = GetEntry(abName, viewName);
            CreateViewAsync(entry, onCreated, onProgress);
            
        }

        protected void CreateViewAsync(Type type, Action<AView> onCreated, Action<float> onProgress)
        {
            ViewEntry entry = GetEntry(type);
            if (null == entry)
            {
                Log.E("没有注册的视图：{0}", type.FullName);
            }
            CreateViewAsync(entry, onCreated, onProgress);            
        }

        void CreateViewAsync(ViewEntry entry, Action<AView> onCreated, Action<float> onProgress)
        {            
            if(null == entry)
            {
                onCreated?.Invoke(null);
                return;
            }

            ResMgr.Ins.LoadAsync(entry.abName, entry.viewName, 
                (UnityEngine.Object asset)=> 
                {
                    GameObject prefab = asset as GameObject;
                    AView view = CreateViewFromPrefab(prefab, _root, entry);
                    onCreated(view);
                }, 
                onProgress);
        }

        protected ViewEntry GetEntry(string abName, string viewName)
        {
            if (false == _ab2view2EntryDic.ContainsKey(abName))
            {
                return null;
            }

            var dic = _ab2view2EntryDic[abName];

            if (false == dic.ContainsKey(viewName))
            {
                return null;
            }

            return dic[viewName];
        }

        protected ViewEntry GetEntry(Type type)
        {
            if(false == _type2EntryDic.ContainsKey(type))
            {
                return null;
            }
            return _type2EntryDic[type];
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
