using System;

namespace ZeroIL.Zero
{
    /// <summary>
    /// UI面板管理器
    /// </summary>
    public class UIPanelMgr:AViewMgr
    {
        /// <summary>
        /// 单例模式
        /// </summary>
        public static readonly UIPanelMgr Ins = new UIPanelMgr();
        private UIPanelMgr()
        {

        }

        AView _nowView;
        string _nowViewName;

        /// <summary>
        /// 切换UIPanel
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <param name="data">传递的数据</param>
        /// <returns></returns>
        public AView Switch(string viewName, object data = null)
        {
            ClearNowPanel();
            //生成新的界面
            var view = CreateView(viewName);
            SetNowView(view, data);
            return view;
        }

        /// <summary>
        /// 切换UIPanel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">传递的数据</param>
        /// <returns></returns>
        public T Switch<T>(object data = null) where T : AView
        {
            ClearNowPanel();
            //生成新的界面
            var view = CreateView(typeof(T));
            SetNowView(view, data);
            return view as T;
        }

        /// <summary>
        /// 异步切换UIPanel
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="data">传递的数据</param>
        /// <param name="onCreated">创建完成回调方法</param>
        /// <param name="onProgress">创建进度回调方法</param>
        public void SwitchASync(string viewName, object data = null, Action<AView> onCreated = null, Action<float> onProgress = null)
        {
            ClearNowPanel();
            CreateViewAsync(viewName, (AView view) => {                
                SetNowView(view, data);
                if (null != onCreated)
                {
                    onCreated(view);
                }
            }, onProgress);
        }

        /// <summary>
        /// 异步切换UIPanel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">传递的数据</param>
        /// <param name="onCreated">创建完成回调方法</param>
        /// <param name="onProgress">创建进度回调方法</param>
        public void SwitchASync<T>(object data = null, Action<AView> onCreated = null, Action<float> onProgress = null)
        {
            ClearNowPanel();
            CreateViewAsync(typeof(T), (AView view) => {                
                SetNowView(view, data);
                if (null != onCreated)
                {
                    onCreated(view);
                }
            }, onProgress);
        }

        /// <summary>
        /// 清理当前的面板
        /// </summary>
        public void ClearNowPanel()
        {
            if (_nowView != null)
            {                
                _nowView.Destroy();
            }
        }

        void SetNowView(AView view, object data = null)
        {
            _nowViewName = view.gameObject.name;
            _nowView = view;
            _nowView.onDestroyHandler += OnViewDestroy;
            view.SetData(data);
        }

        /// <summary>
        /// View对象销毁的回调
        /// </summary>
        /// <param name="view"></param>
        void OnViewDestroy(AView view)
        {
            view.onDestroyHandler -= OnViewDestroy;
            if (_nowView == view)
            {                
                _nowView = null;
                _nowViewName = null;
            }
        }
    }
}
