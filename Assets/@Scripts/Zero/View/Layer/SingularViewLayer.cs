using System;
using UnityEngine;

namespace ZeroHot
{
    /// <summary>
    /// 单一的视图层，该层中的视图，只能存在一个，视图之间的关系是切换
    /// </summary>
    public sealed class SingularViewLayer : AViewLayer
    {
        /// <summary>
        /// 当前的视图
        /// </summary>
        public AView Current { get; private set; }      

        public SingularViewLayer(GameObject rootGameObject) : base(rootGameObject)
        {

        }

        protected override void BeforeShow()
        {
            Clear();
        }

        /// <summary>
        /// 切换视图
        /// </summary>
        /// <param name="view"></param>
        protected override void ShowView(AView view)
        {
            if(null != Current)
            {
                Current.onDestroyed -= OnViewDestroy;
                Current.Destroy();
                Current = null;           
            }

            Current = view;
            view.onDestroyed += OnViewDestroy;
        }

        /// <summary>
        /// View对象销毁的回调
        /// </summary>
        /// <param name="view"></param>
        void OnViewDestroy(AView view)
        {            
            if (Current == view)
            {
                Current.onDestroyed -= OnViewDestroy;
                Current = null;
            }
        }

        public override void Clear()
        {
            if(null != Current)
            {                
                Current.onDestroyed -= OnViewDestroy;
                Current.Destroy();
                Current = null;
            }
        }    
    }
}
