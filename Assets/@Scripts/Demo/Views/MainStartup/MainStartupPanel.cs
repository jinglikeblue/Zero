using System;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroHot;

namespace ILDemo
{
    /// <summary>
    /// 启动面板
    /// </summary>
    public class MainStartupPanel : AView
    {
        Text textState;
        Text textProgress;
        Image bar;

        string[] startupRes = new string[] { "/" };

        protected override void OnInit(object data)
        {
            SetProgress(0, 1);
            ResUpdate update = new ResUpdate(false, false);
            update.Start(startupRes, Enter, OnProgress, OnError);
        }

        private void OnProgress(float progress, long totalSize)
        {
            SetProgress(progress, totalSize);
        }

        private void OnError(string msg)
        {
            textState.text = msg;
        }

        void SetProgress(float progress, long totalSize)
        {
            //转换为MB
            float totalMB = totalSize / 1024 / 1024f;
            float loadedMB = totalMB * progress;
            textProgress.text = string.Format("{0}% [{1}MB/{2}MB]", (int)(progress * 100f), loadedMB.ToString("0.00"), totalMB.ToString("0.00"));
            bar.fillAmount = progress;
        }

        void Enter()
        {
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

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }
    }
}