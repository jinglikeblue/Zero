using System;
using UnityEngine;
using Zero;

namespace ILZero
{
    /// <summary>
    /// 视图创建器
    /// </summary>
    public class ViewAsyncCreater<T> where T : AView
    {
        /// <summary>
        /// 视图的类型
        /// </summary>
        Type _type;

        /// <summary>
        /// 视图所在AB包名称
        /// </summary>
        string _abName;

        /// <summary>
        /// AB包中视图名称
        /// </summary>
        string _viewName;

        /// <summary>
        /// 创建视图所在父节点
        /// </summary>
        Transform _parent;

        /// <summary>
        /// 创建视图附带的数据
        /// </summary>
        object _data;

        /// <summary>
        /// 视图创建成功的回调
        /// </summary>
        Action<T> _onCreated;

        /// <summary>
        /// 视图加载完成的回调
        /// </summary>
        Action<UnityEngine.Object> _onLoaded;             

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">视图类型</param>
        /// <param name="abName">视图所在AB</param>
        /// <param name="viewName">视图名称</param>
        public ViewAsyncCreater(Type type, string abName, string viewName)
        {
            _type = type;
            _abName = abName;
            _viewName = viewName;            
        }

        /// <summary>
        /// 开始创建视图
        /// </summary>
        /// <param name="parent">视图的父节点</param>
        /// <param name="data">创建时传入的数据</param>
        /// <param name="onCreated">创建成功的回调</param>
        /// <param name="onProgress">视图资源加载进度</param>
        /// <param name="onLoaded">视图资源加载完成的回调</param>
        public void Create(Transform parent, object data = null,Action<T> onCreated = null, Action<float> onProgress = null, Action<UnityEngine.Object> onLoaded = null) 
        {
            _parent = parent;
            _data = data;
            _onCreated = onCreated;
            _onLoaded = onLoaded;
            ResMgr.Ins.LoadAsync(_abName, _viewName, OnResLoaded, onProgress);
        }

        private void OnResLoaded(UnityEngine.Object obj)
        {
            if (null != _onLoaded)
            {
                _onLoaded.Invoke(obj);
            }
            var prefab = obj as GameObject;
            var view = ViewFactory.Create(_type, prefab, _parent, _data) as T;
            if (null != _onCreated)
            {
                _onCreated.Invoke(view);
            }
        }
    }
}
