using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Zero
{
    /// <summary>
    /// 游戏预加载逻辑
    /// </summary>
    [DisallowMultipleComponent]
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
        
        public RuntimeVO runtimeCfg = new RuntimeVO();
        
        EState _currentState;
        /// <summary>
        /// 当前状态
        /// </summary>
        public EState CurrentState
        {
            get
            {
                return _currentState;
            }
        }

        /// <summary>
        /// 状态改变的委托
        /// </summary>
        public event Action<EState> onStateChange;
        /// <summary>
        /// 状态对应进度的委托
        /// </summary>
        public event Action<float, long> onProgress;

        /// <summary>
        /// Preload加热失败
        /// </summary>
        public event Action<string> onError;               

        void Start()
        {            

        }

        /// <summary>
        /// 开始激活预加载
        /// </summary>
        /// <param name="rg"></param>
        public void StartPreload(BaseILRuntimeGenerics rg = null)
        {            
            if(null != rg)
            {
                ILRuntimeILWorker.RegisterILRuntimeGenerics(rg);
            }

            //初始化运行环境配置环境
            Runtime.Ins.Init(runtimeCfg);

            Log.CI(Log.COLOR_BLUE, "游戏运行模式：[{0}]", Runtime.Ins.IsHotResProject?Runtime.Ins.ResMode.ToString():"Local");

            if (false == Runtime.Ins.IsHotResProject)
            {
                ResMgr.Ins.Init(ResMgr.EResMgrType.RESOURCES);
                StartMainPrefab();
            }
            else
            {
                OnStageChange(EState.UNZIP_PACKAGE);
                new PackageUpdate().Start(LoadSettingFile, OnPackageUpdate);
            }
        }

        public void OnPackageUpdate(float progress, long totalSize)
        {
            OnProgress(progress, totalSize);
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
            new AppUpdate().Start(StartupResUpdate, OnClientUpdateProgress, OnError);
        }

        private void OnClientUpdateProgress(float progress, long totalSize)
        {
            OnProgress(progress, totalSize);
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

        private void OnUpdateStartupResGroups(float progress, long totalSize)
        {            
            OnProgress(progress, totalSize);
        }

        void StartMainPrefab()
        {            
            OnStageChange(EState.STARTUP);
            GameObject.Destroy(this.gameObject);
            //加载ILRuntimePrefab;            
            GameObject mainPrefab = ResMgr.Ins.Load<GameObject>(ZeroConst.ROOT_AB_FILE_NAME, Runtime.Ins.VO.mainPrefab);
            GameObject go = GameObject.Instantiate(mainPrefab);
            go.name = mainPrefab.name;
        }

        void OnProgress(float progress, long totalSize)
        {
            //Log.W("Progress: {0}", progress);
            if (null != onProgress)
            {
                onProgress.Invoke(progress, totalSize);
            }
        }

        void OnStageChange(EState state)
        {
            _currentState = state;
            Log.W("state: {0}", state);            
            if(null != onStateChange)
            {
                onStateChange.Invoke(state);
            }
                
        }

        /// <summary>
        /// 发生错误
        /// </summary>
        /// <param name="error"></param>
        private void OnError(string error)
        {
            if (null != onError)
            {
                onError.Invoke(error);
            }
        }
    }
}