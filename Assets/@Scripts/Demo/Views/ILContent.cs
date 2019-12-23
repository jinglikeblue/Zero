using ZeroHot;

namespace ILDemo
{
    public class ILContent : AView
    {
        protected override void OnInit(object data)
        {
            base.OnInit(data);
            var rootTransform = gameObject.transform;
            
            var stage = CreateChildView<StageMgr>("Stage");
            var uiPanel = CreateChildView<UIPanelMgr>("UICanvas/UIPanel");
            var uiWin = CreateChildView<UIWinMgr>("UICanvas/UIWin");
        }
    }
}
