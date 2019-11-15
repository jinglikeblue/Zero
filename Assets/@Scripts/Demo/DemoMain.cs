using ZeroHot;
using UnityEngine;
using Zero;

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
            UIPanelMgr.Ins.SwitchASync<MenuPanel>();
            //播放背景音乐
            AudioDevice.Get("bgm").Play(ResMgr.Ins.Load<AudioClip>("audios/bgm"), true);
        }
    }
}
