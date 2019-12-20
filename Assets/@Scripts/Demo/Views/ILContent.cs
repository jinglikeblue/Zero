using System;
using ZeroHot;

namespace ILDemo
{
    public class ILContent : AView
    {
        protected override void OnInit(object data)
        {
            base.OnInit(data);
            var rootTransform = gameObject.transform;
            var stageRoot = rootTransform.Find("Stage");
            var uiPanelRoot = rootTransform.Find("UICanvas/UIPanel");
            var uiWinRoot = rootTransform.Find("UICanvas/UIWin");

            StageMgr.Ins.Init(stageRoot);
            UIPanelMgr.Ins.Init(uiPanelRoot);
            UIWinMgr.Ins.Init(uiWinRoot);
        }
    }
}
