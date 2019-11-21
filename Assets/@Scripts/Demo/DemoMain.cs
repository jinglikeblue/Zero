using ZeroHot;
using UnityEngine;
using Zero;
using System;

namespace ILDemo
{
    public class DemoMain
    {
        public DemoMain()
        {
            Init();
            Go();
        }

        void Init()
        {
            Application.targetFrameRate = 60;

            AudioDevice.Create("bgm");
            AudioDevice.Create("effect");

            //加载ILRuntimePrefab;
            GameObject mainPrefab = ResMgr.Ins.Load<GameObject>(AB.ROOT_ASSETS.NAME, AB.ROOT_ASSETS.ILContent);
            var ilContent = ViewFactory.Create<ILContent>(mainPrefab, null);
            ilContent.gameObject.name = mainPrefab.name;
        }

        public void Go()
        {
            //Log.GUI("This demo is code by Jing");
            GUIDeviceInfo.Show();
            UIPanelMgr.Ins.SwitchASync<MenuPanel>(null, OnCreated, OnProgress);
            //播放背景音乐
            AudioDevice.Get("bgm").Play(ResMgr.Ins.Load<AudioClip>("audios/bgm"), true);
        }

        private void OnProgress(float progress)
        {
            Debug.Log("加载进度：" + progress);
        }

        private void OnCreated(AView view)
        {
            Debug.Log("创建完成:" + view.gameObject.name);
        }
    }
}
