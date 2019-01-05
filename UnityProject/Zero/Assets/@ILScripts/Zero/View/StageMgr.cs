using Zero;
using System;

namespace IL.Zero
{
    /// <summary>
    /// 舞台管理器
    /// </summary>
    public class StageMgr:AViewMgr
    {
        /// <summary>
        /// 单例模式
        /// </summary>
        private static StageMgr _ins = new StageMgr();
        public static StageMgr Ins
        {
            get { return _ins;}
        }

        private StageMgr()
        {

        }

        AView _nowView;

        /// <summary>
        /// 切换Stage
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <param name="data">传递的数据</param>
        /// <param name="isClearPanel">是否清理UIPanel</param>
        /// <param name="isCloseWindows">是否清理UIWin</param>
        /// <returns></returns>
        public AView Switch(string abName, string viewName, object data = null)
        {
            ClearNowStage();
            //生成新的界面
            var view = ViewFactory.Create(abName, viewName, _root, data);
            SetNowView(view);
            return view;
        }

        /// <summary>
        /// 切换Stage
        /// </summary>
        /// <typeparam name="T">对应Stage的AView对象</typeparam>
        /// <param name="data">传递的数据</param>
        /// <param name="isClearPanel">是否清理UIPanel</param>
        /// <param name="isCloseWindows">是否清理UIWin</param>
        /// <returns></returns>
        public T Switch<T>(object data = null, bool isClearPanel = true, bool isCloseWindows = true) where T : AView
        {
            ClearNowStage();
            //生成新的界面
            var view = ViewFactory.Create(typeof(T), _root, data);
            SetNowView(view);
            return view as T;
        }

        Action<AView> _onAsyncCreated;

        /// <summary>
        /// 异步切换场景
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <param name="data">传递的数据</param>
        /// <param name="onCreated">创建完成回调方法</param>
        /// <param name="onProgress">创建进度回调方法</param>
        /// <param name="isClearPanel">是否清理UIPanel</param>
        /// <param name="isCloseWindows">是否清理UIWin</param>
        public void SwitchASync(string abName, string viewName, object data = null, Action<AView> onCreated = null, Action<float> onProgress = null, bool isClearPanel = true, bool isCloseWindows = true)
        {
            _onAsyncCreated = onCreated;
            ViewFactory.CreateAsync(abName, viewName, _root, data, OnAsyncCreated, onProgress, ClearNowStage);
        }

        private void OnAsyncCreated(AView view)
        {
            SetNowView(view);
            _onAsyncCreated?.Invoke(view);
        }

        /// <summary>
        /// 异步切换场景
        /// </summary>
        /// <typeparam name="T">对应Stage的AView对象</typeparam>
        /// <param name="data">传递的数据</param>
        /// <param name="onCreated">创建完成回调方法</param>
        /// <param name="onProgress">创建进度回调方法</param>
        /// <param name="isClearPanel">是否清理UIPanel</param>
        /// <param name="isCloseWindows">是否清理UIWin</param>
        public void SwitchASync<T>(object data = null, Action<AView> onCreated = null, Action<float> onProgress = null, bool isClearPanel = true, bool isCloseWindows = true)
        {
            _onAsyncCreated = onCreated;
            ViewFactory.CreateAsync(typeof(T), _root, data, OnAsyncCreated, onProgress, ClearNowStage);
        }

        void SetNowView(AView view)
        {
            _nowView = view;
            _nowView.onDestroyHandler += OnViewDestroy;

            //执行一次垃圾回收
            ResMgr.Ins.DoGC();
        }

        /// <summary>
        /// 清理当前的舞台
        /// </summary>
        public void ClearNowStage()
        {
            if (_nowView != null)
            {
                _nowView.Destroy();
            }
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
