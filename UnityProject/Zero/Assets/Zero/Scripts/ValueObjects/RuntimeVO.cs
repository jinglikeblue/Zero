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
    public class RuntimeVO 
    {     
        [Header("是否允许打印日志")]
        public bool isLogEnable;

        [Header("启动Prefab路径")]
        public string mainPrefab;

        [Header("是否是热更项目")]
        public bool isHotResProject;

        [Header("资源的来源")]
        public EHotResMode hotResMode;

        [Header("网络资源的根目录")]
        public string netRoot;

        [Header("本地资源的根目录")]
        public string localResRoot;

        #region 脚本热更部分
        [Header("只使用DLL(忽略反射执行代码)")]
        public bool isUseDll;

        [Header("是否通过反射执行DLL(限Mono)")]
        public EILType ilType = EILType.IL_RUNTIME;

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
        #endregion
    }
}