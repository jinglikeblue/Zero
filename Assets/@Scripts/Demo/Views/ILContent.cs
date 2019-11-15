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


            //

            //RegistStages();
            //RegistUIPanel();
            //RegistUIWin();

            StageMgr.Ins.Init(stageRoot);
            UIPanelMgr.Ins.Init(uiPanelRoot);
            UIWinMgr.Ins.Init(uiWinRoot);
        }

        void RegistStages()
        {
            RegistView<GameStage>(AB.PREFABS_STAGES.NAME);
        }

        void RegistUIPanel()
        {
            RegistView<MenuPanel>(AB.PREFABS_PANELS.NAME);
            RegistView<GamePanel>(AB.PREFABS_PANELS.NAME);
        }

        void RegistUIWin()
        {
            RegistView<HelpWin>(AB.PREFABS_WINS.NAME);
        }

        void RegistView<T>(string abName)
        {
            Type t = typeof(T);
            ViewFactory.Register(abName, t.Name, t);
        }
    }
}
