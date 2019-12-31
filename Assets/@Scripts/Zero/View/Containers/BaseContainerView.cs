using System;

namespace ZeroHot
{
    /// <summary>
    /// 基础容器类
    /// </summary>
    public abstract class BaseContainerView : AView
    {
        /// <summary>
        /// 显示视图(使用该方式显示视图，请先在ViewFactory中注册AViewType)
        /// </summary>
        /// <typeparam name="AViewType"></typeparam>
        /// <param name="data">传递的数据</param>
        /// <returns></returns>
        public AViewType Show<AViewType>(object data = null) where AViewType : AView
        {
            BeforeShow();
            var view = ViewFactory.Create<AViewType>(transform, data);
            ShowView(view);
            return view;
        }

        /// <summary>
        /// 显示视图(使用该方式显示视图，请先在ViewFactory中注册type)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public AView Show(Type type, object data = null)
        {
            BeforeShow();
            var view = ViewFactory.Create(type, transform, data);
            ShowView(view);
            return view;
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
            BaseContainerView _layer;
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

            public void Begin(BaseContainerView layer, object data, Action<AViewType, object> onCreated, object token, Action<float> onProgress, Action<UnityEngine.Object> onLoaded)
            {
                _layer = layer;
                _onCreated = onCreated;
                _token = token;
                ViewFactory.CreateAsync(_viewType, layer.transform, data, OnAsyncCreated, onProgress, onLoaded);
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
                    _onCreated.Invoke((AViewType)view, _token);
                }
            }
        }
    }
}
