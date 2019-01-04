using System;
using System.Collections.Generic;
using UnityEngine;
using Zero;

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
        Blur _blur;

        public Blur Blur
        {
            get
            {
                if (null != _blur)
                {
                    return _blur;
                }
                return null;
            }
        }

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
            var blurGO = root.Find("Blur");           
            if (null != blurGO)
            {
                _blur = blurGO.GetComponent<Blur>();
                if (null != _blur)
                {
                    _blur.gameObject.SetActive(false);
                }
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
            if (isCloseOthers)
            {
                CloseAll();
            }

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
        public AView Open(string abName, string viewName, object data = null, bool isBlur = true, bool isCloseOthers = true)
        {
            if (isCloseOthers)
            {
                CloseAll();
            }

            var view = CreateView(abName, viewName);
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
            if (isCloseOthers)
            {
                CloseAll();
            }

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
        public void OpenAsync(string abName, string viewName, object data = null, bool isBlur = true, bool isCloseOthers = true, Action<AView> onCreated = null, Action<float> onProgress = null)
        {
            if (isCloseOthers)
            {
                CloseAll();
            }

            CreateViewAsync(abName, viewName, (AView view) =>
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
            _nowWindows.Add(view);
            _nowWindows.Sort(ComparerView);
            view.onDestroyHandler += OnViewDestroy;
            view.SetData(data);

            if (isBlur)
            {
                _needBlurViewSet.Add(view);                
                UpdateBlur();
            }
        }

        private int ComparerView(AView x, AView y)
        {
            int xIdx = x.GameObject.transform.GetSiblingIndex();
            int yIdx = y.GameObject.transform.GetSiblingIndex();
            return xIdx - yIdx;
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
                        int viewChildIdx = view.GameObject.transform.GetSiblingIndex();
                        int blurChildIdx = _blur.transform.GetSiblingIndex();
                        if(blurChildIdx < viewChildIdx)
                        {
                            viewChildIdx--;
                        }
                        _blur.transform.SetSiblingIndex(viewChildIdx);
                        return;
                    }
                }
            }
            else
            {
                _blur.gameObject.SetActive(false);
            }
            _blur.transform.SetSiblingIndex(_blur.transform.parent.childCount - 2);
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
