using System;
using UnityEngine;
using Zero;

namespace IL.Zero
{
    /// <summary>
    /// 视图创建器
    /// </summary>
    public class ViewAsyncCreater<T> where T : AView
    {
        Type _type;
        string _abName;
        string _viewName;

        Transform _parent;
        object _data;
        Action<T> _onCreated;        

        public ViewAsyncCreater(Type type, string abName, string viewName)
        {
            _type = type;
            _abName = abName;
            _viewName = viewName;            
        }

        public void Create(Transform parent, object data = null,Action<T> onCreated = null, Action<float> onProgress = null) 
        {
            _parent = parent;
            _data = data;
            _onCreated = onCreated;            
            ResMgr.Ins.LoadAsync(_abName, _viewName, OnLoaded, onProgress);
        }

        private void OnLoaded(UnityEngine.Object obj)
        {
            var prefab = obj as GameObject;
            var view = ViewFactory.Create(_type, prefab, _parent, _data) as T;
            _onCreated?.Invoke(view);
        }
    }
}
