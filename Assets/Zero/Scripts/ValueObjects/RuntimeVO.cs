using Sirenix.OdinInspector;
using System;
using System.Collections;

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
        [InfoBox("当项目为非热更项目时，ResMgr只能请求Resources目录中的资源", InfoMessageType = InfoMessageType.Info)]
        [LabelText("项目是否使用热更")]
        public bool isHotResProject;

        [SuffixLabel("关闭日志打印可以提高执行效率")]        
        [LabelText("是否允许打印日志")]
        public bool isLogEnable;

        [Title("启动")]
        [LabelText("启动类(完全限定类名)"), DisplayAsString]
        public string className = ZeroConst.LOGIC_SCRIPT_STARTUP_CLASS_NAME;

        [LabelText("启动方法(静态)"), DisplayAsString]
        public string methodName = ZeroConst.LOGIC_SCRIPT_STARTUP_METHOD;

        [Title("热更配置")]
        [InfoBox("$HotResSource", InfoMessageType = InfoMessageType.None)]
        [LabelText("资源的来源"), ValueDropdown("HotResMode"), ShowIf("isHotResProject")]
        public EHotResMode hotResMode;
        
        [LabelText("网络资源的根目录"), ShowIf("isHotResProject"), ShowIf("hotResMode", EHotResMode.NET_ASSET_BUNDLE)]
        public string netRoot;

        //[LabelText("本地资源的根目录"), ShowIf("isHotResProject"), DisplayAsString, ShowIf("hotResMode", EHotResMode.LOCAL_ASSET_BUNDLE)]
        //public string localResRoot = ZeroConst.PUBLISH_RES_ROOT_DIR;

        //[LabelText("Asset中热更资源的根目录"), ShowIf("isHotResProject"), DisplayAsString, ShowIf("hotResMode", EHotResMode.ASSET_DATA_BASE)]
        //public string hotResRoot = ZeroConst.HOT_RESOURCES_ROOT_DIR;

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

        string HotResSource()
        {
            string source = "资源来源:    {0}";
            switch (hotResMode)
            {
                case EHotResMode.NET_ASSET_BUNDLE:
                    source = string.Format(source, Log.Zero2("网络资源根目录"));
                    break;
                case EHotResMode.LOCAL_ASSET_BUNDLE:
                    source = string.Format(source, Log.Zero2(ZeroConst.PUBLISH_RES_ROOT_DIR));
                    break;
                case EHotResMode.ASSET_DATA_BASE:
                    source = string.Format(source, Log.Zero2(ZeroConst.HOT_RESOURCES_ROOT_DIR));
                    break;
            }
            return source;
        }

        void OpenHotResSource()
        {

        }

        IEnumerable ILType = new ValueDropdownList<EILType>()
        {
            { "ILRuntime框架", EILType.IL_RUNTIME },
            { "反射执行(IL2CPP下会自动切换为ILRuntime)", EILType.REFLECTION },
        };
#endif
    }
}