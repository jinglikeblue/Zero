using System;
using System.Collections.Generic;
using UnityEngine;

namespace IL.Zero
{
    /// <summary>
    /// 窗口管理器
    /// </summary>
    public class UIWinMgr : AViewMgr
    {
        /// <summary>
        /// 单例模式
        /// </summary>
        public static readonly UIWinMgr Ins = new UIWinMgr();

        List<AView> _nowWindows = new List<AView>();

        /// <summary>
        /// 窗口隔离遮罩
        /// </summary>
        Transform _blur;

        /// <summary>
        /// 需要有遮罩的窗口
        /// </summary>
        HashSet<AView> _needBlurViewSet = new HashSet<AView>();



        private UIWinMgr()
        {

        }

        public override void Init(Transform root)
        {
            base.Init(root);
            _blur = root.Find("Blur");
            if (null != _blur)
            {
                _blur.gameObject.SetActive(false);
            }
        }



        /// <summary>
        /// 打开窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">传递的数据</param>
        /// <param name="isBlur">是否窗口下方有阻挡遮罩</param>
        /// <param name="isCloseOthers">是否关闭其它窗口</param>
        /// <returns></returns>
        public T Open<T>(object data = null, bool isBlur = true, bool isCloseOthers = true) where T : AView
        {
            var view = CreateView(typeof(T));
            OnCreateView(view, data, isBlur, isCloseOthers);
            return view as T;
        }

        /// <summary>
        /// 打开窗口
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <param name="data">传递的数据</param>
        /// <param name="isBlur">是否窗口下方有阻挡遮罩</param>
        /// <param name="isCloseOthers">是否关闭其它窗口</param>
        /// <returns></returns>
        public AView Open(string viewName, object data = null, bool isBlur = true, bool isCloseOthers = true)
        {
            var view = CreateView(viewName);
            OnCreateView(view, data, isBlur, isCloseOthers);
            return view;
        }

        /// <summary>
        /// 异步打开窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">传递的数据</param>
        /// <param name="isBlur">是否窗口下方有阻挡遮罩</param>
        /// <param name="isCloseOthers">是否关闭其它窗口</param>
        /// <param name="onCreated">创建完成回调方法</param>
        /// <param name="onProgress">创建进度回调方法</param>
        public void OpenAsync<T>(object data = null, bool isBlur = true, bool isCloseOthers = true, Action<AView> onCreated = null, Action<float> onProgress = null)
        {
            CreateViewAsync(typeof(T), (AView view) =>
            {
                OnCreateView(view, data, isBlur, isCloseOthers);
                if (null != onCreated)
                {
                    onCreated(view);
                }
            }, onProgress);
        }

        /// <summary>
        /// 异步打开窗口
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <param name="data">传递的数据</param>
        /// <param name="isBlur">是否窗口下方有阻挡遮罩</param>
        /// <param name="isCloseOthers">是否关闭其它窗口</param>
        /// <param name="onCreated">创建完成回调方法</param>
        /// <param name="onProgress">创建进度回调方法</param>
        public void OpenAsync(string viewName, object data = null, bool isBlur = true, bool isCloseOthers = true, Action<AView> onCreated = null, Action<float> onProgress = null)
        {
            CreateViewAsync(viewName, (AView view) =>
            {
                OnCreateView(view, data, isBlur, isCloseOthers);
                if (null != onCreated)
                {
                    onCreated(view);
                }
            }, onProgress);
        }

        void OnCreateView(AView view, object data, bool isBlur, bool isCloseOthers)
        {
            if (isCloseOthers)
            {
                CloseAll();
            }

            _nowWindows.Add(view);
            view.onDestroyHandler += OnViewDestroy;
            view.SetData(data);

            if (isBlur)
            {
                _needBlurViewSet.Add(view);                
                UpdateBlur();
            }
        }

        void UpdateBlur()
        {
            if (null == _blur)
            {
                return;
            }

            if (_needBlurViewSet.Count > 0)
            {
                _blur.gameObject.SetActive(true);                
                for (int i = _nowWindows.Count - 1; i > -1; i--)
                {
                    AView view = _nowWindows[i];                    
                    if (_needBlurViewSet.Contains(view))
                    {
                        int viewChildIdx = view.gameObject.transform.GetSiblingIndex();
                        int blurChildIdx = _blur.GetSiblingIndex();
                        if(blurChildIdx < viewChildIdx)
                        {
                            viewChildIdx--;
                        }
                        _blur.SetSiblingIndex(viewChildIdx);
                        return;
                    }
                }
            }
            else
            {
                _blur.gameObject.SetActive(false);
            }
            _blur.SetSiblingIndex(_blur.parent.childCount - 2);
        }

        /// <summary>
        /// View对象销毁的回调
        /// </summary>
        /// <param name="view"></param>
        void OnViewDestroy(AView view)
        {
            view.onDestroyHandler -= OnViewDestroy;

            _nowWindows.Remove(view);
            _needBlurViewSet.Remove(view);

            UpdateBlur();
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="target">关闭对象</param>
        public void Close(AView target)
        {            
            target.Destroy();          
        }

        /// <summary>
        /// 关闭(当前打开的)所有窗口
        /// </summary>
        public void CloseAll()
        {
            _needBlurViewSet.Clear();
            int count = _nowWindows.Count;
            for (int i = 0; i < count; i++)
            {
                AView view = _nowWindows[i];
                view.onDestroyHandler -= OnViewDestroy;
                view.Destroy();
            }
            _nowWindows.RemoveRange(0, count);
            UpdateBlur();
        }
    }
}
