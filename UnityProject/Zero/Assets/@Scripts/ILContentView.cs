using ILDemo;
using ILGenerated;
using ILZero;
using System;
using UnityEngine;
using Zero;

namespace IL
{
    public class ILContentView : AView
    {
        protected override void OnInit()
        {
            base.OnInit();
            var rootTransform = gameObject.transform;
            var stageRoot = rootTransform.Find("Stage");
            var uiPanelRoot = rootTransform.Find("UICanvas/UIPanel");
            var uiWinRoot = rootTransform.Find("UICanvas/UIWin");


            RegistStages();
            RegistUIPanel();
            RegistUIWin();

            StageMgr.Ins.Init(stageRoot);
            UIPanelMgr.Ins.Init(uiPanelRoot);
            UIWinMgr.Ins.Init(uiWinRoot);
        }

        void RegistStages()
        {
            RegistView<GameStage>(AssetBundleName.PREFABS_STAGES);
        }

        void RegistUIPanel()
        {
            RegistView<MenuPanel>(AssetBundleName.PREFABS_PANELS);
            RegistView<GamePanel>(AssetBundleName.PREFABS_PANELS);
        }

        void RegistUIWin()
        {
            RegistView<HelpWin>(AssetBundleName.PREFABS_WINS);
        }

        void RegistView<T>(string abName)
        {
            Type t = typeof(T);
            ViewFactory.Register(abName, t.Name, t);
        }
    }
}
