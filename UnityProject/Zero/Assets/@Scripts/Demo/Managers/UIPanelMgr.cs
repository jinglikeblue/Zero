using ILZero;
using System;
using System.Collections;
using UnityEngine;
using Zero;

namespace ILDemo
{
    /// <summary>
    /// UI面板管理器
    /// </summary>
    public class UIPanelMgr : ASingleton<UIPanelMgr>
    {
        SingularViewLayer _layer;

        public void Init(Transform root)
        {
            if (null == _layer)
            {
                _layer = new SingularViewLayer(root.gameObject);
            }
        }

        /// <summary>
        /// 切换UIPanel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">传递的数据</param>
        /// <returns></returns>
        public T Switch<T>(object data = null) where T : AView
        {
            _layer.Clear();
            //生成新的界面
            var view = _layer.Show<T>(data);
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
            _layer.ShowASync<T>(data, OnASyncShow, onCreated, onProgress);                        
        }

        private void OnASyncShow(AView view, object token)
        {
            var onCreated = token as Action<AView>;
            if (null != onCreated)
            {
                onCreated.Invoke(view);
            }
        }

        /// <summary>
        /// 清理当前的面板
        /// </summary>
        public void Clear()
        {
            _layer.Clear();
        }
    }
}
