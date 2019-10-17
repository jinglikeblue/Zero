using ILGenerated;
using ILZero;
using System;
using UnityEngine;
using Zero;

namespace ILDemo
{
    public class DemoMain
    {
        public DemoMain()
        {
            Init();
            RegistStages();
            RegistUIPanel();
            RegistUIWin();
            Go();
        }

        void Init()
        {
            Application.targetFrameRate = 60;

            AudioDevice.Create("bgm");
            AudioDevice.Create("effect");

            var ILContent = GameObject.Find("ILContent");
            var stageRoot = ILContent.transform.Find("Stage");
            var uiPanelRoot = ILContent.transform.Find("UICanvas/UIPanel");
            var uiWinRoot = ILContent.transform.Find("UICanvas/UIWin");

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

        public void Go()
        {
            //Log.GUI("This demo is code by Jing");
            GUIDeviceInfo.Show();
            UIPanelMgr.Ins.SwitchASync<MenuPanel>();
            //播放背景音乐
            AudioDevice.Get("bgm").Play(ResMgr.Ins.Load<AudioClip>("audios/bgm"), true);
        }
    }
}
