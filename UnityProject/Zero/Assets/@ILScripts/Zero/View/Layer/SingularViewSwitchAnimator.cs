using System;

namespace IL.Zero
{
    /// <summary>
    /// 单一视图切换的动画控制器
    /// </summary>
    public abstract class ASingularViewSwitchAnimator
    {
        public AView Old;

        /// <summary>
        /// 开始切换动画
        /// </summary>
        /// <param name="oldView">旧视图</param>
        /// <param name="newView">新视图</param>
        /// <param name="onSwitchComplete">切换完成后必须调用该方法，才可以正确销毁旧的视图</param>
        public abstract void StartSwitch(AView oldView, AView newView, Action<ASingularViewSwitchAnimator> onSwitchComplete);
    }
}
