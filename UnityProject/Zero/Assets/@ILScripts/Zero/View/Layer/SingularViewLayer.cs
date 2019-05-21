using System;
using UnityEngine;

namespace IL.Zero
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

        /// <summary>
        /// 切换视图
        /// </summary>
        /// <param name="view"></param>
        public override void ShowView(AView view)
        {
            if(null != Current)
            {
                Current.onDestroyHandler -= OnViewDestroy;
                Current.Destroy();
                Current = null;           
            }

            Current = view;
            view.onDestroyHandler += OnViewDestroy;
        }

        /// <summary>
        /// View对象销毁的回调
        /// </summary>
        /// <param name="view"></param>
        void OnViewDestroy(AView view)
        {            
            if (Current == view)
            {
                Current.onDestroyHandler -= OnViewDestroy;
                Current = null;
            }
        }

        public override void Clear()
        {
            if(null != Current)
            {                
                Current.onDestroyHandler -= OnViewDestroy;
                Current.Destroy();
                Current = null;
            }
        }
    }
}
