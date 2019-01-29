using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 运行时数据对象
    /// </summary>
    [Serializable]
    public struct RuntimeVO 
    {       
        /// <summary>
        /// 资源使用模式
        /// </summary>
        public enum EResMode
        {
            /// <summary>
            /// 内嵌资源项目-发布模式
            /// </summary>
            INLINE_RELEASE,
            /// <summary>
            /// 网络资源项目-发布模式
            /// </summary>            
            NET_RELEASE,
            /// <summary>
            /// 网络资源项目-开发模式
            /// <para></para>
            /// 本地加载资源的开发模式 
            /// </summary>            
            NET_LOCAL_DEBUG,
            /// <summary>
            /// 网络资源项目-开发模式
            /// <para></para>
            /// 警告：需要打包AB的资源如果放在Resources目录下才可选择
            /// <para></para>
            /// 当需要使用AB资源的时候，资源管理器会自动从Resources目录下进行读取，这样修改AB资源可以立刻调试不用重新Build，提高开发效率
            /// </summary>            
            NET_LOCAL_AND_RESOURCES_DEBUG
        }

        /// <summary>
        /// IL配置
        /// </summary>
        [Serializable]
        public struct ILCfgVO
        {
            [Header("只使用DLL(忽略反射执行代码)")]
            public bool isOnlyDll;

            [Header("是否通过反射执行DLL(限Mono)")]
            public bool isReflection;
            [Header("文件目录（相对于资源存储位置）")]
            public string fileDir;

            [Header("文件名(不含后缀名)")]
            public string fileName;

            [Header("启动类")]
            public string className;

            [Header("Lib文件启动方法(必须为Static)")]
            public string methodName;

            [Header("IL是否调试模式")]
            public bool isDebugIL;

            [Header("IL是否加载Pdb文件")]
            public bool isLoadPdb;
        }

        [Serializable]
        public struct MainPrefabCfgVO
        {
            [Header("资源文件名称")]
            public string abName;

            [Header("prefab名称")]
            public string assetName;
        }


        [Header("资源的来源")]
        public EResMode resMode;

        [Header("网络资源的根目录")]
        public string netRoot;

        [Header("开发资源的根目录")]
        public string developResRoot;

        [Header("是否允许打印日志")]
        public bool logEnable;

        [Header("il配置")]
        public ILCfgVO ilCfg;

        [Header("启动配置")]
        public MainPrefabCfgVO mainPrefab;        
    }
}