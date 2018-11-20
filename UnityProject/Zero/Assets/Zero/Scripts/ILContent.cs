using System;
using System.Reflection;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 中间代码内容
    /// </summary>
    public class ILContent : MonoBehaviour
    {
        string _runMode;

        private void Start()
        {
            var rt = Runtime.Ins;
            var ilCfg = rt.ILCfg;

            
            string qualifiedName = Assembly.CreateQualifiedName(Runtime.Ins.ILCfg.fileName, ilCfg.className);
            Type type = Type.GetType(ilCfg.className);
            if(ilCfg.isOnlyDll || type == null)
            {
                string libDir = rt.localResDir + ilCfg.fileDir;
                //初始化IL
                ILRuntimeBridge.Ins.Startup(libDir, ilCfg.fileName, ilCfg.isDebugIL, ilCfg.isLoadPdb);
                //调用启动方法
                ILRuntimeBridge.Ins.Invoke(ilCfg.className, ilCfg.methodName);

                _runMode = "外部程序集";
            }
            else
            {
                //使用本地类，直接启动本地类
                MethodInfo method = type.GetMethod(ilCfg.methodName, BindingFlags.Static | BindingFlags.Public);
                method.Invoke(null, null);

                _runMode = "本地程序集";
            }

            Log.CI(Log.COLOR_BLUE, "IL代码运行环境: [{0}]", _runMode);
        }
    }
}