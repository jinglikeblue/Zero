using System;
using ZeroHot;

namespace ILDemo
{
    public class PanelContainerView: SingularContainerView
    {
        /// <summary>
        /// 切换UIPanel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">传递的数据</param>
        /// <returns></returns>
        public T Switch<T>(object data = null) where T : AView
        {
            Clear();
            //生成新的界面
            var view = Show<T>(data);
            return view;
        }

        /// <summary>
        /// 切换UIPanel
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data">传递的数据</param>
        /// <returns></returns>
        public AView Switch(Type type, object data = null)
        {
            Clear();
            //生成新的界面
            var view = Show(type, data);
            return view;
        }

        /// <summary>
        /// 异步切换UIPanel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">传递的数据</param>
        /// <param name="onCreated">创建完成回调方法</param>
        /// <param name="onProgress">创建进度回调方法</param>
        public void SwitchASync<T>(object data = null, Action<AView> onCreated = null, Action<float> onProgress = null) where T : AView
        {
            ShowASync<T>(data, OnASyncShow, onCreated, onProgress);
        }

        /// <summary>
        /// 异步切换UIPanel
        /// </summary>
        /// <param name="data">传递的数据</param>
        /// <param name="onCreated">创建完成回调方法</param>
        /// <param name="onProgress">创建进度回调方法</param>
        public void SwitchASync(Type type, object data = null, Action<AView> onCreated = null, Action<float> onProgress = null)
        {
            ShowASync(type, data, OnASyncShow, onCreated, onProgress);
        }

        private void OnASyncShow(AView view, object token)
        {
            var onCreated = token as Action<AView>;
            if (null != onCreated)
            {
                onCreated.Invoke(view);
            }
        }
    }
}
