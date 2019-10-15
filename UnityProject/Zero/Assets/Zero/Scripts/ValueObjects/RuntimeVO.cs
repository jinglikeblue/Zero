using Sirenix.OdinInspector;
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
    [HideLabel]
    public class RuntimeVO
    {
        [Title("基础")]
        [InfoBox("当项目为非热更项目时，ResMgr只能请求Resources目录中的资源", InfoMessageType = InfoMessageType.Warning)]
        [LabelText("项目是否使用热更")]
        public bool isHotResProject;

        [LabelText("是否允许打印日志")]
        public bool isLogEnable;

        [Title("启动")]
        [LabelText("启动Prefab路径"), DisableInEditorMode, DisableInPlayMode]
        public string mainPrefab = @"ILContent";

        [LabelText("启动类(完全限定类名)"), DisableInEditorMode, DisableInPlayMode]
        public string className;

        [LabelText("启动方法(静态)"), DisableInEditorMode, DisableInPlayMode]
        public string methodName;

        [Title("热更配置")]
        [LabelText("资源的来源"), ValueDropdown("HotResMode"), ShowIf("isHotResProject")]
        public EHotResMode hotResMode;

        [LabelText("网络资源的根目录"), ShowIf("isHotResProject"), ShowIf("hotResMode", EHotResMode.NET_ASSET_BUNDLE)]
        public string netRoot;

        [LabelText("本地资源的根目录"), ShowIf("isHotResProject"), DisableInEditorMode, DisableInPlayMode, ShowIf("hotResMode", EHotResMode.LOCAL_ASSET_BUNDLE)]
        public string localResRoot = "请查看「ZeroEditorConst.PUBLISH_RES_ROOT_DIR」";

        [LabelText("Asset中热更资源的根目录"), ShowIf("isHotResProject"), DisableInEditorMode, DisableInPlayMode, ShowIf("hotResMode", EHotResMode.ASSET_DATA_BASE)]
        public string hotResRoot = ZeroConst.HOT_RESOURCES_ROOT_DIR;

        [Title("热更代码配置")]
        [LabelText("使用DLL"), ShowIf("isHotResProject")]
        public bool isUseDll;

        [LabelText("DLL执行方式"), ValueDropdown("ILType"), ShowIf("isHotResProject"), ShowIf("isUseDll")]
        public EILType ilType = EILType.IL_RUNTIME;

        [LabelText("调试功能"), ShowIf("isHotResProject"), ShowIf("ilType", EILType.IL_RUNTIME), ShowIf("isUseDll")]
        public bool isDebugIL;

        [LabelText("加载Pdb文件"), ShowIf("isHotResProject"), ShowIf("ilType", EILType.IL_RUNTIME), ShowIf("isUseDll")]
        public bool isLoadPdb;

#if UNITY_EDITOR
        IEnumerable HotResMode = new ValueDropdownList<EHotResMode>()
        {
            { "从网络资源目录加载资源", EHotResMode.NET_ASSET_BUNDLE },
            { "从本地资源目录加载资源", EHotResMode.LOCAL_ASSET_BUNDLE },
            { "使用AssetDataBase加载资源（限开发阶段使用）", EHotResMode.ASSET_DATA_BASE },
        };

        IEnumerable ILType = new ValueDropdownList<EILType>()
        {
            { "ILRuntime框架", EILType.IL_RUNTIME },
            { "反射执行(IL2CPP下会自动切换为ILRuntime)", EILType.REFLECTION },
        };
#endif
    }
}