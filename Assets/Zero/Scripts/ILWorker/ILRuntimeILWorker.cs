using System;
using System.IO;
using UnityEngine;

namespace Zero
{
    public class ILRuntimeILWorker : BaseILWorker
    {
        public ILRuntime.Runtime.Enviorment.AppDomain appDomain
        {
            get
            {                
                return _appdomain;
            }
        }

        //AppDomain是ILRuntime的入口，最好是在一个单例类中保存，整个游戏全局就一个，这里为了示例方便，每个例子里面都单独做了一个
        //大家在正式项目中请全局只创建一个AppDomain
        ILRuntime.Runtime.Enviorment.AppDomain _appdomain = null;

        public ILRuntimeILWorker(byte[] dllBytes, string libDir, string libName, bool isDebug, bool isNeedPdbFile)
        {
            Debug.Log(Log.Zero1("外部程序集执行方式：[ILRuntime]"));

            //首先实例化ILRuntime的AppDomain，AppDomain是一个应用程序域，每个AppDomain都是一个独立的沙盒
            _appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();

            if (isDebug)
            {
                //启动调试监听
                _appdomain.DebugService.StartDebugService(56000);
            }

            MemoryStream fs = new MemoryStream(dllBytes);

            if (isNeedPdbFile)
            {
                string pdbPath = Path.Combine(libDir, libName + ".pdb");
                byte[] pdbBytes = File.ReadAllBytes(pdbPath);                
                MemoryStream p = new MemoryStream(pdbBytes);
                _appdomain.LoadAssembly(fs, p, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
            }
            else
            {                
                _appdomain.LoadAssembly(fs);
            }

            InitializeILRuntime();
        }

        void InitializeILRuntime()
        {
            var appdomain = _appdomain;

            //这里做一些Zero依赖的的ILRuntime的注册
            new ILRuntimeRegisters(appdomain).Register();

            //进行CLR绑定。通过反射执行，这样如果没有绑定代码，也不会报错
            var classCLRBinding = Type.GetType("ILRuntime.Runtime.Generated.CLRBindings");
            if(null != classCLRBinding)
            {
                //注册CLR绑定            
                var methodInitialize = classCLRBinding.GetMethod("Initialize");
                if(null != methodInitialize)
                {
                    methodInitialize.Invoke(null, new object[] { appdomain });
                }                
            }
        }

        public override void Invoke(string clsName, string methodName)
        {
            _appdomain.Invoke(clsName, methodName, null, null);
        }
    }
}