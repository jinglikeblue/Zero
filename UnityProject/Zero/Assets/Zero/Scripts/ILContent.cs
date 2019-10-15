using Jing;
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
            var cfg = Runtime.Ins.VO;
            if(null == cfg)
            {
                return;
            }
            bool isUseDll = cfg.isUseDll && cfg.isHotResProject;
            
            Type type = Type.GetType(cfg.className);
            if (isUseDll || type == null)
            {
                Log.CI(Log.COLOR_ORANGE, "IL代码运行环境: [外部程序集]");

                string dllDir = FileSystem.CombineDirs(false, Runtime.Ins.localResDir, ZeroConst.DLL_DIR_NAME);
                //初始化IL
                ILBridge.Ins.Startup(dllDir, ZeroConst.DLL_FILE_NAME, cfg.isDebugIL, cfg.isLoadPdb);
                //调用启动方法
                ILBridge.Ins.Invoke(cfg.className, cfg.methodName);
            }
            else
            {
                Log.CI(Log.COLOR_ORANGE, "IL代码运行环境: [本地程序集]");

                //使用本地类，直接启动本地类
                MethodInfo method = type.GetMethod(cfg.methodName, BindingFlags.Static | BindingFlags.Public);
                method.Invoke(null, null);
            }
        }
    }
}