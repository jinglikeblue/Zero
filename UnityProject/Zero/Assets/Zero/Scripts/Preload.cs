using Jing;
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

        /// <summary>
        /// 开始激活预加载
        /// </summary>
        /// <param name="rg"></param>
        public void StartPreload()
        {            
            //初始化运行环境配置环境
            Runtime.Ins.Init(runtimeCfg);

            Debug.Log(Log.Zero1("游戏运行模式：[{0}]", Runtime.Ins.IsHotResProject?Runtime.Ins.ResMode.ToString():"Local"));

            if (false == Runtime.Ins.IsHotResProject)
            {
                ResMgr.Ins.Init(ResMgr.EResMgrType.RESOURCES);
                RunScripts();
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
            update.Start(Runtime.Ins.setting.startupResGroups, RunScripts, OnUpdateStartupResGroups, onError);
        }

        private void OnUpdateStartupResGroups(float progress, long totalSize)
        {            
            OnProgress(progress, totalSize);
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
            Debug.LogWarningFormat("state: {0}", state);            
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

        void RunScripts()
        {
            OnStageChange(EState.STARTUP);
            GameObject.Destroy(this.gameObject);

            var cfg = Runtime.Ins.VO;
            bool isUseDll = cfg.isUseDll && cfg.isHotResProject;
            
            if (isUseDll)
            {
                Debug.Log(Log.Zero1("@Scripts代码运行环境: [外部程序集]"));

                string dllDir = FileSystem.CombineDirs(false, Runtime.Ins.localResDir, ZeroConst.DLL_DIR_NAME);
                //初始化IL
                ILBridge.Ins.Startup(dllDir, ZeroConst.DLL_FILE_NAME, cfg.isDebugIL, cfg.isLoadPdb);
                //调用启动方法
                ILBridge.Ins.Invoke(cfg.className, cfg.methodName);
            }
            else
            {
                Debug.Log(Log.Zero1("@Scripts代码运行环境: [本地程序集]"));

                Type type = Type.GetType(cfg.className);
                //使用本地类，直接启动本地类
                MethodInfo method = type.GetMethod(cfg.methodName, BindingFlags.Static | BindingFlags.Public);
                method.Invoke(null, null);
            }
        }
    }
}