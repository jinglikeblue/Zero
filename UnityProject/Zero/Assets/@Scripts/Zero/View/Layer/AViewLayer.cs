using System;
using UnityEngine;

namespace ILZero
{
    public abstract class AViewLayer
    {
        private GameObject _rootGO;

        /// <summary>
        /// 根节点容器
        /// </summary>
        protected Transform Root
        {
            get
            {
                return _rootGO.transform;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootGameObject">作为根节点的GameObject对象</param>
        public AViewLayer(GameObject rootGameObject)
        {
            _rootGO = rootGameObject;
        }

        /// <summary>
        /// 显示视图(使用该方式显示视图，请先在ViewFactory中注册AViewType)
        /// </summary>
        /// <typeparam name="AViewType"></typeparam>
        /// <param name="data">传递的数据</param>
        /// <returns></returns>
        public AViewType Show<AViewType>(object data = null) where AViewType : AView
        {
            BeforeShow();
            var view = ViewFactory.Create<AViewType>(Root, data);
            ShowView(view);
            return view as AViewType;
        }

        /// <summary>
        /// 异步显示视图(使用该方式显示视图，请先在ViewFactory中注册AViewType)
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="data">传递的数据</param>
        /// <param name="onCreated">创建完成回调方法，会传回显示的视图以及token对象</param>
        /// <param name="onProgress">创建进度回调方法</param>
        public ASyncShow<AViewType> ShowASync<AViewType>(object data = null, Action<AViewType, object> onCreated = null, object token = null, Action<float> onProgress = null, Action<UnityEngine.Object> onLoaded = null) where AViewType : AView
        {
            var show = new ASyncShow<AViewType>();
            show.Begin(this, data, onCreated, token, onProgress, (obj) =>
            {                
                onLoaded?.Invoke(obj);
                BeforeShow();
            });
            return show;
        }

        public ASyncShow<AView> ShowASync(Type type, object data = null, Action<AView, object> onCreated = null, object token = null, Action<float> onProgress = null, Action<UnityEngine.Object> onLoaded = null)
        {
            var show = new ASyncShow<AView>(type);
            show.Begin(this, data, onCreated, token, onProgress, (obj) =>
            {                
                onLoaded?.Invoke(obj);
                BeforeShow();
            });
            return show;
        }

        /// <summary>
        /// 当资源准备好准备开始生成视图对象前被调用
        /// </summary>
        /// <param name="view"></param>
        protected abstract void BeforeShow();

        /// <summary>
        /// 显示视图
        /// </summary>
        /// <param name="view"></param>
        protected abstract void ShowView(AView view);

        /// <summary>
        /// 清理视图
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// 异步展示处理类
        /// </summary>
        /// <typeparam name="AViewType"></typeparam>
        public class ASyncShow<AViewType> where AViewType : AView
        {
            AViewLayer _layer;
            Action<AViewType, object> _onCreated;
            object _token;
            Type _viewType;

            public ASyncShow()
            {
                _viewType = typeof(AViewType);
            }

            public ASyncShow(Type type)
            {
                _viewType = type;
            }

            public void Begin(AViewLayer layer, object data, Action<AViewType, object> onCreated, object token, Action<float> onProgress, Action<UnityEngine.Object> onLoaded)
            {
                _layer = layer;
                _onCreated = onCreated;
                _token = token;                
                ViewFactory.CreateAsync(_viewType, layer.Root, data, OnAsyncCreated, onProgress, onLoaded);
            }

            /// <summary>
            /// 界面异步创建完成
            /// </summary>
            /// <param name="view"></param>
            private void OnAsyncCreated(AView view)
            {
                _layer.ShowView(view);
                if (null != _onCreated)
                {
                    _onCreated.Invoke(view as AViewType, _token);
                }
            }
        }
    }
}
