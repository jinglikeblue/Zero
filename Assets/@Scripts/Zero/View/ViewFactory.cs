using System;
using System.Collections.Generic;
using UnityEngine;
using Zero;

namespace ZeroHot
{
    /// <summary>
    /// 视图工厂
    /// </summary>
    public sealed class ViewFactory
    {
        /// <summary>
        /// 视图单元的数据
        /// </summary>
        struct ViewEntry
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

        static Dictionary<string, Dictionary<string, ViewEntry>> _ab2view2EntryDic = new Dictionary<string, Dictionary<string, ViewEntry>>();
        static Dictionary<Type, ViewEntry> _type2EntryDic = new Dictionary<Type, ViewEntry>();

        /// <summary>
        /// 注册一个界面
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="viewName"></param>
        /// <param name="type">Prefab的Type</param>
        static public void Register(string abName, string viewName, Type type)
        {            
            ViewEntry entry = new ViewEntry(abName, viewName, type);
            if (false == _ab2view2EntryDic.ContainsKey(abName))
            {
                _ab2view2EntryDic[abName] = new Dictionary<string, ViewEntry>();
            }

            _ab2view2EntryDic[abName][viewName] = entry;
            _type2EntryDic[type] = entry;
        }

        /// <summary>
        /// 注册一个界面
        /// </summary>
        /// <typeparam name="AViewType">AView类</typeparam>
        /// <param name="abName">Prefab所在AssetBundle的名称</param>
        /// <param name="viewName">Prefab的名称</param>
        static public void Register<AViewType>(string abName, string viewName) where AViewType:AView
        {
            Register(abName, viewName, typeof(AViewType));
        }



        /// <summary>
        /// 创建视图
        /// </summary>
        /// <param name="type"></param>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static AView Create(Type type, GameObject prefab, Transform parent, object data = null)
        {
            GameObject go = GameObject.Instantiate(prefab, parent);
            go.name = prefab.name;

            AView view = Activator.CreateInstance(type) as AView;
            view.SetGameObject(go, data);
            return view;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T Create<T>(GameObject prefab, Transform parent, object data = null) where T : AView
        {
            AView view = Create(typeof(T), prefab, parent, data);
            return view as T;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="abName"></param>
        /// <param name="viewName"></param>
        /// <param name="parent"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static AView Create(Type type, string abName, string viewName, Transform parent, object data = null)
        {
            GameObject prefab = ResMgr.Ins.Load<GameObject>(abName, viewName);
            return Create(type, prefab, parent, data);
        }

        public static T Create<T>(string abName, string viewName, Transform parent, object data = null) where T : AView
        {
            GameObject prefab = ResMgr.Ins.Load<GameObject>(abName, viewName);            
            return Create<T>(prefab, parent, data);
        }

        public static AView Create(Type type, Transform parent, object data = null)
        {
            if(_type2EntryDic.ContainsKey(type))
            {
                ViewEntry ve = _type2EntryDic[type];
                return Create(ve.type, ve.abName, ve.viewName, parent, data);
            }
            return null;
        }

        public static T Create<T>(Transform parent, object data = null) where T : AView
        {
            Type type = typeof(T);
            return Create(type, parent, data) as T;
        }

        public static AView Create(string abName, string viewName, Transform parent, object data = null)
        {
            if (_ab2view2EntryDic.ContainsKey(abName))
            {
                var v2eDic = _ab2view2EntryDic[abName];
                if (v2eDic.ContainsKey(viewName))
                {
                    ViewEntry ve = v2eDic[viewName];
                    return Create(ve.type, ve.abName, ve.viewName, parent, data);
                }
            }

            return null;            
        }       

        public static void CreateAsync(Type type, string abName, string viewName, Transform parent, object data = null, Action<AView> onCreated = null, Action<float> onProgress = null, Action<UnityEngine.Object> onLoaded = null)
        {
            new ViewAsyncCreater<AView>(type, abName, viewName).Create(parent,data,onCreated,onProgress, onLoaded);
        }

        public static void CreateAsync<T>(string abName, string viewName, Transform parent, object data = null, Action<T> onCreated = null, Action<float> onProgress = null, Action<UnityEngine.Object> onLoaded = null) where T : AView
        {
            new ViewAsyncCreater<T>(typeof(T), abName, viewName).Create(parent, data, onCreated, onProgress, onLoaded);
        }

        public static void CreateAsync(Type type, Transform parent, object data = null, Action<AView> onCreated = null, Action<float> onProgress = null, Action<UnityEngine.Object> onLoaded = null)
        {            
            if (_type2EntryDic.ContainsKey(type))
            {
                ViewEntry ve = _type2EntryDic[type];
                new ViewAsyncCreater<AView>(ve.type, ve.abName, ve.viewName).Create(parent, data, onCreated, onProgress, onLoaded);
            }
        }

        public static void CreateAsync<T>(Transform parent, object data = null, Action<T> onCreated = null, Action<float> onProgress = null, Action<UnityEngine.Object> onLoaded = null) where T : AView
        {
            Type type = typeof(T);
            if (_type2EntryDic.ContainsKey(type))
            {
                ViewEntry ve = _type2EntryDic[type];
                new ViewAsyncCreater<T>(ve.type, ve.abName, ve.viewName).Create(parent, data, onCreated, onProgress, onLoaded);
            }
        }

        public static void CreateAsync(string abName, string viewName, Transform parent, object data = null, Action<AView> onCreated = null, Action<float> onProgress = null, Action<UnityEngine.Object> onLoaded = null)
        {
            if (_ab2view2EntryDic.ContainsKey(abName))
            {
                var v2eDic = _ab2view2EntryDic[abName];
                if (v2eDic.ContainsKey(viewName))
                {
                    ViewEntry ve = v2eDic[viewName];
                    new ViewAsyncCreater<AView>(ve.type, ve.abName, ve.viewName).Create(parent, data, onCreated, onProgress, onLoaded);
                }
            }
        }
    }
}
