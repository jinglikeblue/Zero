using System;
using UnityEngine;

namespace IL.Zero
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
            var view = ViewFactory.Create<AViewType>(Root, data);
            ShowView(view);
            return view as AViewType;
        }

        /// <summary>
        /// 异步显示视图(使用该方式显示视图，请先在ViewFactory中注册AViewType)
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="data">传递的数据</param>
        /// <param name="onCreated">创建完成回调方法</param>
        /// <param name="onProgress">创建进度回调方法</param>
        public void ShowASync<AViewType>(object data = null, Action<AViewType> onCreated = null, Action<float> onProgress = null) where AViewType : AView
        {
            var show = new ASyncShow<AViewType>();
            show.Begin(this, data, onCreated, onProgress);
        }

        /// <summary>
        /// 显示视图
        /// </summary>
        /// <param name="view"></param>
        public abstract void ShowView(AView view);


        /// <summary>
        /// 异步展示处理类
        /// </summary>
        /// <typeparam name="AViewType"></typeparam>
        class ASyncShow<AViewType> where AViewType : AView
        {
            AViewLayer _layer;
            Action<AViewType> _onCreated;

            public void Begin(AViewLayer layer, object data, Action<AViewType> onCreated, Action<float> onProgress)
            {
                _layer = layer;
                _onCreated = onCreated;
                ViewFactory.CreateAsync(typeof(AViewType), layer.Root, data, OnAsyncCreated, onProgress);
            }

            /// <summary>
            /// 界面异步创建完成
            /// </summary>
            /// <param name="view"></param>
            private void OnAsyncCreated(AView view)
            {
                _layer.ShowView(view);
                _onCreated?.Invoke(view as AViewType);
            }
        }
    }
}
