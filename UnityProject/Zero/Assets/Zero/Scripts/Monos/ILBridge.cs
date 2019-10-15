using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Zero;

namespace Zero
{
    /// <summary>
    /// IL代码执行桥接器。如果可以通过反射获取动态代码，则通过反射执行。否则采用ILRuntime框架执行。
    /// </summary>
    public class ILBridge : ASingletonMonoBehaviour<ILBridge>
    {
        public string dllDir;
        public string dllName;

        /// <summary>
        /// Update事件委托
        /// </summary>
        public event Action onUpdate;

        /// <summary>
        /// OnGUI事件委托
        /// </summary>
        public event Action onGUI;

        /// <summary>
        /// OnFixedUpdate事件委托
        /// </summary>
        public event Action onFixedUpdate;

        /// <summary>
        /// OnLateUpdate事件委托
        /// </summary>
        public event Action onLateUpdate;

        /// <summary>
        /// 客户端焦点事件
        /// </summary>
        public event Action<bool> onApplicationFocus;

        /// <summary>
        /// 客户端暂停事件
        /// </summary>
        public event Action<bool> onApplicationPause;

        /// <summary>
        /// 程序退出
        /// </summary>
        public event Action onApplicationQuit;

        /// <summary>
        /// IL代码执行的工作器
        /// </summary>
        BaseILWorker iLWorker;

        /// <summary>
        /// 启动热更代码执行
        /// </summary>
        /// <param name="dllDir">Dll文件所在目录</param>
        /// <param name="dllName">DLL文件名称（不含扩展名）</param>
        /// <param name="isDebug">是否是调试模式（仅针对ILRuntime，可以用第三方插件在APP运行时进行调试）</param>
        /// <param name="methodName">是否需要加载PDB文件（仅针对ILRuntime，可以在调试时打印出错代码信息）</param>
        public void Startup(string dllDir, string dllName, bool isDebug, bool isNeedPdbFile)
        {
            this.dllDir = dllDir;
            this.dllName = dllName;

            string dllPath = Path.Combine(dllDir, dllName + ".dll");

            byte[] dllBytes = File.ReadAllBytes(dllPath);

            Assembly assembly = null;

            //开发时，优先保证代码在ILRuntime下能够正常运行
            if (Runtime.Ins.VO.ilType == EILType.REFLECTION)
            {
                assembly = AssemblyILWorker.LoadAssembly(dllBytes);
            }

            if (null != assembly)
            {
                //使用Assembly
                iLWorker = new AssemblyILWorker(assembly);
            }
            else
            {
                //使用ILRuntime
                iLWorker = new ILRuntimeILWorker(dllBytes, dllDir, dllName, isDebug, isNeedPdbFile);
            }
        }

        public void Invoke(string clsName, string methodName)
        {
            if (null != iLWorker)
            {
                iLWorker.Invoke(clsName, methodName);
            }
        }

        private void OnGUI()
        {
            if (null != onGUI)
            {
                onGUI.Invoke();
            }
        }

        void Update()
        {
            if (null != onUpdate)
            {
                onUpdate.Invoke();
            }
        }

        private void FixedUpdate()
        {
            if (null != onFixedUpdate)
            {
                onFixedUpdate.Invoke();
            }
        }

        private void LateUpdate()
        {
            if (null != onLateUpdate)
            {
                onLateUpdate.Invoke();
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (null != onApplicationFocus)
            {
                onApplicationFocus.Invoke(focus);
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (null != onApplicationPause)
            {
                onApplicationPause.Invoke(pause);
            }
        }

        private void OnApplicationQuit()
        {
            if (null != onApplicationQuit)
            {
                onApplicationQuit.Invoke();
            }
        }
    }
}