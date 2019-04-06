using System;
using UnityEngine;

namespace IL.Zero
{
    /// <summary>
    /// 单一的视图层，该层中的视图，只能存在一个，视图之间的关系是切换
    /// （
    /// 可以根据自己的需求实现视图的切换控制器。
    /// 两个视图的切换时会使内存的占用为旧视图的内存以及新视图的内存之和。
    /// 如果对内存占用要求较高，则可以考虑增加一个loading视图来作为视图切换的中介。
    /// ）
    /// </summary>
    public sealed class SingularViewLayer : AViewLayer
    {
        /// <summary>
        /// 当前的视图
        /// </summary>
        public AView Current { get; private set; }

        Type _switchAnimatorType;        

        public SingularViewLayer(GameObject rootGameObject) : base(rootGameObject)
        {

        }

        /// <summary>
        /// 注册视图切换动画控制器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RegistSwitchAnimator(ASingularViewSwitchAnimator animator)
        {
            if (null == animator)
            {
                _switchAnimatorType = null;
            }
            else
            {
                _switchAnimatorType = animator.GetType();
            }
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
                //检查是否存在切换效果，没有则直接销毁老的界面
                if (null == _switchAnimatorType)
                {
                    Current.Destroy();
                    Current = null;
                }
                else
                {
                    var animator = (ASingularViewSwitchAnimator)Activator.CreateInstance(_switchAnimatorType);
                    animator.Old = Current;
                    animator.StartSwitch(Current, view, OnSwitchComplete);
                }               
            }

            Current = view;
            view.onDestroyHandler += OnViewDestroy;
        }

        void OnSwitchComplete(ASingularViewSwitchAnimator animator)
        {
            animator.Old.Destroy();
            animator = null;
        }

        /// <summary>
        /// View对象销毁的回调
        /// </summary>
        /// <param name="view"></param>
        void OnViewDestroy(AView view)
        {
            view.onDestroyHandler -= OnViewDestroy;
            if (Current == view)
            {
                Current = null;
            }
        }

        public override void Clear()
        {
            if(null != Current)
            {
                var view = Current;
                OnViewDestroy(Current);
                view.Destroy();
            }
        }
    }
}
