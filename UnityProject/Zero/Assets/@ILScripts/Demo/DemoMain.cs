using IL.Zero;
using System;
using UnityEngine;
using Zero;

namespace IL.Demo
{
    class DemoMain
    {
        /// <summary>
        /// 入口方法
        /// </summary>
        public static void Main()
        {
            new DemoMain();
        }

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
            var ILContent = GameObject.Find("DemoILContent");
            var stageRoot = ILContent.transform.Find("Stage");
            var uiPanelRoot = ILContent.transform.Find("UICanvas/UIPanel");
            var uiWinRoot = ILContent.transform.Find("UICanvas/UIWin");

            StageMgr.Ins.Init(stageRoot);
            UIPanelMgr.Ins.Init(uiPanelRoot);
            UIWinMgr.Ins.Init(uiWinRoot);
        }


        void RegistStages()
        {
            RegistView<GameStage>("hot_res/prefabs/stages");            
        }

        void RegistUIPanel()
        {
            RegistView<MenuPanel>("hot_res/prefabs/panels");
            RegistView<GamePanel>("hot_res/prefabs/panels");
        }

        void RegistUIWin()
        {
            RegistView<HelpWin>("hot_res/prefabs/wins");
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
        }
    }
}
