using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Zero
{
    /// <summary>
    /// 游戏预加载逻辑
    /// </summary>
    public class Preload : MonoBehaviour
    {
        public enum EState
        {
            /// <summary>
            /// 解压StreamingAssets/Package.zip
            /// </summary>
            UNZIP_PACKAGE,
            /// <summary>
            /// 更新Setting.json
            /// </summary>
            SETTING_UPDATE,
            /// <summary>
            /// 客户端更新
            /// </summary>
            CLIENT_UDPATE,
            /// <summary>
            /// 资源更新
            /// </summary>
            RES_UPDATE,
            /// <summary>
            /// 启动主程序
            /// </summary>
            STARTUP
        }

        [Header("运行时配置")]
        public RuntimeVO runtimeCfg;

        /// <summary>
        /// 状态改变的委托
        /// </summary>
        public event Action<EState> onStateChange;
        /// <summary>
        /// 状态对应进度的委托
        /// </summary>
        public event Action<float> onProgress;

        /// <summary>
        /// Preload加热失败
        /// </summary>
        public event Action<string> onError;
        void Start()
        {            

        }

        public void StartPreload(BaseILRuntimeGenerics rg = null)
        {
            if(null != rg)
            {
                ILRuntimeILWorker.RegisterILRuntimeGenerics(rg);
            }

            Runtime.Ins.Init(runtimeCfg);
            Log.CI(Log.COLOR_BLUE, "游戏运行模式：[{0}]", Runtime.Ins.ResMode.ToString());
            if (Runtime.Ins.IsInlineRelease)
            {
                ResMgr.Ins.Init(ResMgr.EResMgrType.RESOURCES, Runtime.Ins.VO.mainPrefab.abName);
                StartMainPrefab();
            }
            else
            {
                OnStageChange(EState.UNZIP_PACKAGE);
                new PackageUpdate().Start(LoadSettingFile, OnPackageUpdate);
            }
        }

        public void OnPackageUpdate(float progress)
        {
            OnProgress(progress);
        }

        void LoadSettingFile()
        {
            OnStageChange(EState.SETTING_UPDATE);
            new SettingUpdate().Start(ClientUpdate, OnError);
        }

        /// <summary>
        /// 客户端更新
        /// </summary>
        void ClientUpdate()
        {                       
            OnStageChange(EState.CLIENT_UDPATE);
            AClientUpdate.CreateNowPlatformUpdate().Start(StartupResUpdate, OnClientUpdateProgress, OnError);
        }

        private void OnClientUpdateProgress(float progress)
        {
            OnProgress(progress);
        }

        /// <summary>
        /// 更新初始化所需资源
        /// </summary>
        void StartupResUpdate(bool isOverVersion)
        {
            OnStageChange(EState.RES_UPDATE);           

            ResUpdate update = new ResUpdate();
            update.Start(Runtime.Ins.setting.startupResGroups, StartMainPrefab, OnUpdateStartupResGroups, onError);
        }

        private void OnUpdateStartupResGroups(float progress)
        {
            OnProgress(progress);
        }

        void StartMainPrefab()
        {            
            OnStageChange(EState.STARTUP);
            GameObject.Destroy(this.gameObject);
            //加载ILRuntimePrefab;            
            GameObject mainPrefab = ResMgr.Ins.Load<GameObject>(Runtime.Ins.VO.mainPrefab.abName, Runtime.Ins.VO.mainPrefab.assetName);
            GameObject go = GameObject.Instantiate(mainPrefab);
            go.name = Runtime.Ins.VO.mainPrefab.assetName;
        }

        void OnProgress(float progress)
        {
            //Log.W("Progress: {0}", progress);
            if (null != onProgress)
            {
                onProgress.Invoke(progress);
            }
        }

        void OnStageChange(EState state)
        {
            Log.W("Stage: {0}", state);            
            onStateChange?.Invoke(state);            
        }

        /// <summary>
        /// 发生错误
        /// </summary>
        /// <param name="error"></param>
        private void OnError(string error)
        {
            onError?.Invoke(error);
        }
    }
}