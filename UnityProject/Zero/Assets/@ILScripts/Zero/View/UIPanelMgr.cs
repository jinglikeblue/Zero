using System;

namespace IL.Zero
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

        /// <summary>
        /// 切换UIPanel
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <param name="data">传递的数据</param>
        /// <returns></returns>
        public AView Switch(string abName, string viewName, object data = null)
        {
            ClearNowPanel();
            //生成新的界面
            var view =  ViewFactory.Create(abName, viewName, _root, data);
            SetNowView(view);
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
            var view = ViewFactory.Create(typeof(T),_root, data);
            SetNowView(view);
            return view as T;
        }

        Action<AView> _onAsyncCreated;

        /// <summary>
        /// 异步切换UIPanel
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="data">传递的数据</param>
        /// <param name="onCreated">创建完成回调方法</param>
        /// <param name="onProgress">创建进度回调方法</param>
        public void SwitchASync(string abName, string viewName, object data = null, Action<AView> onCreated = null, Action<float> onProgress = null)
        {            
            _onAsyncCreated = onCreated;
            ViewFactory.CreateAsync(abName, viewName,_root, data, OnAsyncCreated, onProgress, ClearNowPanel);
        }

        private void OnAsyncCreated(AView view)
        {
            SetNowView(view);
            _onAsyncCreated?.Invoke(view);
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
            _onAsyncCreated = onCreated;
            ViewFactory.CreateAsync(typeof(T), _root, data, OnAsyncCreated, onProgress, ClearNowPanel);
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

        void SetNowView(AView view)
        {
            _nowView = view;
            _nowView.onDestroyHandler += OnViewDestroy;
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
            }
        }
    }
}
