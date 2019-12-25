using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Zero
{
    public class ILRuntimeILWorker : BaseILWorker
    {
        /// <summary>
        /// appDomain是ILRuntime的入口，最好是在一个单例类中保存，整个游戏全局就一个，这里为了示例方便，每个例子里面都单独做了一个
        /// <para>大家在正式项目中请全局只创建一个AppDomain</para>
        /// </summary>
        public ILRuntime.Runtime.Enviorment.AppDomain appDomain { get; }

        /// <summary>
        /// DLL文件所在目录
        /// </summary>
        public string dllDir { get; private set; }

        /// <summary>
        /// DLL文件名称(没有扩展名)
        /// </summary>
        public string dllNameWithoutExt { get; private set; }

        ILRuntime.CLR.TypeSystem.IType[] _ilRuntimeTypes;

        public ILRuntimeILWorker(byte[] dllBytes, string dllDir, string dllNameWithoutExt, bool isDebug, bool isNeedPdbFile)
        {            
            this.dllDir = dllDir;
            this.dllNameWithoutExt = dllNameWithoutExt;
            //首先实例化ILRuntime的AppDomain，AppDomain是一个应用程序域，每个AppDomain都是一个独立的沙盒
            appDomain = new ILRuntime.Runtime.Enviorment.AppDomain();

            if (isDebug)
            {
                //启动调试监听
                appDomain.DebugService.StartDebugService(56000);
            }

            MemoryStream fs = new MemoryStream(dllBytes);

            if (isNeedPdbFile)
            {
                string pdbPath = Path.Combine(dllDir, dllNameWithoutExt + ".pdb");
                byte[] pdbBytes = File.ReadAllBytes(pdbPath);                
                MemoryStream p = new MemoryStream(pdbBytes);
                appDomain.LoadAssembly(fs, p, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
            }
            else
            {                
                appDomain.LoadAssembly(fs);
            }

            InitializeILRuntime();

            _ilRuntimeTypes = appDomain.LoadedTypes.Values.ToArray();            
        }

        void InitializeILRuntime()
        {
            var appdomain = appDomain;

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
            appDomain.Invoke(clsName, methodName, null, null);
        }

        /// <summary>
        /// 获取ILRuntime中的IType类型清单
        /// </summary>
        /// <returns></returns>
        public ILRuntime.CLR.TypeSystem.IType[] GetILRuntimeTypes()
        {           
            ILRuntime.CLR.TypeSystem.IType[] types = new ILRuntime.CLR.TypeSystem.IType[_ilRuntimeTypes.Length];
            Array.Copy(_ilRuntimeTypes, types, types.Length);
            return types;
        }

        public override Type[] GetTypes(Func<Type, bool> whereFunc = null)
        {
            Type[] types;

            var typeList = new List<Type>();
            foreach (var ilType in _ilRuntimeTypes)
            {
                if (null == whereFunc || whereFunc.Invoke(ilType.ReflectionType))
                {
                    typeList.Add(ilType.ReflectionType);
                }
            }
            types = typeList.ToArray();
            return types;
        }
    }
}