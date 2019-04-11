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
        private void Start()
        {
            var rt = Runtime.Ins;
            var ilCfg = rt.ILCfg;
            
            string qualifiedName = Assembly.CreateQualifiedName(Runtime.Ins.ILCfg.fileName, ilCfg.className); 
            Type type = Type.GetType(ilCfg.className);
            if(ilCfg.isUseDll || type == null)
            {
                Log.CI(Log.COLOR_ORANGE, "IL代码运行环境: [外部程序集]");

                string libDir = rt.localResDir + ilCfg.fileDir;
                //初始化IL
                ILBridge.Ins.Startup(libDir, ilCfg.fileName, ilCfg.isDebugIL, ilCfg.isLoadPdb);
                //调用启动方法
                ILBridge.Ins.Invoke(ilCfg.className, ilCfg.methodName);
            }
            else
            {
                Log.CI(Log.COLOR_ORANGE, "IL代码运行环境: [本地程序集]");

                //使用本地类，直接启动本地类
                MethodInfo method = type.GetMethod(ilCfg.methodName, BindingFlags.Static | BindingFlags.Public);
                method.Invoke(null, null);                
            }            
        }
    }
}