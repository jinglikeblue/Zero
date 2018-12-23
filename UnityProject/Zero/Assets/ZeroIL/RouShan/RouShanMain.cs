using System;
using UnityEngine;
using Zero;
using ZeroIL.Zero;

namespace ZeroIL.RouShan
{
    class RouShanMain
    {
        /// <summary>
        /// 入口方法
        /// </summary>
        public static void Main()
        {
            new RouShanMain();
        }

        public RouShanMain()
        {
            Init();
            RegistStages();
            RegistUIPanel();
            RegistUIWin();
            Go();
        }

        void Init()
        {
            //Application.targetFrameRate = 60;
            var ILContent = GameObject.Find(Runtime.Ins.VO.mainPrefab.assetName);
            var stageRoot = ILContent.transform.Find("Stage");
            var uiPanelRoot = ILContent.transform.Find("UICanvas/UIPanel");
            var uiWinRoot = ILContent.transform.Find("UICanvas/UIWin");

            StageMgr.Ins.Init(stageRoot);
            UIPanelMgr.Ins.Init(uiPanelRoot);
            UIWinMgr.Ins.Init(uiWinRoot);
        }


        void RegistStages()
        {
            RegistView<GameStage>("hot_res/prefabs/stages.ab");            
        }

        void RegistUIPanel()
        {
            RegistView<MenuPanel>("hot_res/prefabs/panels.ab");
            RegistView<GamePanel>("hot_res/prefabs/panels.ab");
        }

        void RegistUIWin()
        {
            RegistView<HelpWin>("hot_res/prefabs/wins.ab");
        }

        void RegistView<T>(string abName)
        {
            Type t = typeof(T);
            AViewMgr.Regist(t.Name, abName, t);
        }

        public void Go()
        {
            Log.GUI("This demo is code by Jing");
            GUIDeviceInfo.Show();
            UIPanelMgr.Ins.Switch<MenuPanel>();                                   
        }
    }
}
