using ZeroHot;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zero;

namespace ILDemo
{
    /// <summary>
    /// 窗口管理器
    /// </summary>
    public class UIWinMgr:ASingleton<UIWinMgr>
    {
        struct WinSetting
        {
            public bool isBlur;
            public bool isCloseOthers;
            public Action<AView> onCreated;

            public WinSetting(bool isBlur, bool isCloseOthers, Action<AView> onCreated)
            {
                this.isBlur = isBlur;
                this.isCloseOthers = isCloseOthers;
                this.onCreated = onCreated;
            }
        }

        /// <summary>
        /// 需要有遮罩的窗口
        /// </summary>
        HashSet<AView> _needBlurViewSet = new HashSet<AView>();

        PluralViewLayer _layer;

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

        public void Init(Transform root)
        {
            if (null == _layer)
            {
                _layer = new PluralViewLayer(root.gameObject);
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
            var view = _layer.Show<T>(data);
            OnShowView(view, isBlur, isCloseOthers);
            return view as T;
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
        public void OpenAsync<T>(object data = null, bool isBlur = true, bool isCloseOthers = true, Action<AView> onCreated = null, Action<float> onProgress = null) where T : AView
        {
            if (isCloseOthers)
            {
                CloseAll();
            }
            _layer.ShowASync<T>(data, OnAsyncOpen, new WinSetting(isBlur, isCloseOthers, onCreated), onProgress);
        }

        private void OnAsyncOpen(AView view, object token)
        {
            var setting = (WinSetting)token;

            OnShowView(view, setting.isBlur, setting.isCloseOthers);
            if (null != setting.onCreated)
            {
                setting.onCreated.Invoke(view);
            }
        }

        void OnShowView(AView view, bool isBlur, bool isCloseOthers)
        {
            _layer.ViewList.Sort(ComparerView);
            view.onDestroyed += OnViewDestroy;

            if (isBlur)
            {
                _needBlurViewSet.Add(view);
                UpdateBlur();
            }
        }

        private int ComparerView(AView x, AView y)
        {
            int xIdx = x.gameObject.transform.GetSiblingIndex();
            int yIdx = y.gameObject.transform.GetSiblingIndex();
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
                for (int i = _layer.ViewList.Count - 1; i > -1; i--)
                {
                    AView view = _layer.ViewList[i];
                    if (_needBlurViewSet.Contains(view))
                    {
                        int viewChildIdx = view.gameObject.transform.GetSiblingIndex();
                        int blurChildIdx = _blur.transform.GetSiblingIndex();
                        if (blurChildIdx < viewChildIdx)
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
            view.onDestroyed -= OnViewDestroy;
            _needBlurViewSet.Remove(view);
            UpdateBlur();
        }

        /// <summary>
        /// 关闭(当前打开的)所有窗口
        /// </summary>
        public void CloseAll()
        {
            foreach (var view in _layer.ViewList)
            {
                view.onDestroyed -= OnViewDestroy;
            }
            _needBlurViewSet.Clear();
            _blur.gameObject.transform.SetAsFirstSibling();
            _blur.gameObject.SetActive(false);
            _layer.Clear();
        }

        public override void Destroy()
        {
            throw new NotImplementedException();
        }
    }
}
