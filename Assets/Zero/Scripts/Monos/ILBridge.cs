using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// IL代码执行桥接器。如果可以通过反射获取动态代码，则通过反射执行。否则采用ILRuntime框架执行。
    /// </summary>
    public class ILBridge : ASingletonMonoBehaviour<ILBridge>
    {
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
        /// 是否是ILRuntime模式
        /// </summary>
        public bool IsILRuntimeMode { get; private set; } = false;

        /// <summary>
        /// 当ILRuntime模式时存在值
        /// </summary>
        public ILRuntime.Runtime.Enviorment.AppDomain ILRuntimeAppDomain { get; private set; }

        /// <summary>
        /// 获取代码域中的类型清单
        /// </summary>
        /// <param name="whereFunc">可选参数，委托通过参数Type判断是否需要加入清单中，返回true则表示需要</param>
        /// <returns></returns>
        public Type[] GetTypes(Func<Type, bool> whereFunc = null)
        {
            return iLWorker.GetTypes(whereFunc);
        }

        public void Startup()
        {
            //使用Assembly
            iLWorker = new AssemblyILWorker(this.GetType().Assembly);
        }

        /// <summary>
        /// 启动热更代码执行
        /// </summary>
        /// <param name="dllDir">Dll文件所在目录</param>
        /// <param name="dllName">DLL文件名称（不含扩展名）</param>
        /// <param name="isDebug">是否是调试模式（仅针对ILRuntime，可以用第三方插件在APP运行时进行调试）</param>
        /// <param name="methodName">是否需要加载PDB文件（仅针对ILRuntime，可以在调试时打印出错代码信息）</param>
        public void Startup(string dllDir, string dllName, bool isDebug, bool isNeedPdbFile)
        {
            string dllPath = Path.Combine(dllDir, dllName + ".dll");

            byte[] dllBytes = File.ReadAllBytes(dllPath);

            Assembly assembly = null;

            //如果是反射执行模式，则优先检查是否反射执行，如果不行则切换为ILRuntime模式
            if (Runtime.Ins.VO.ilType == EILType.REFLECTION)
            {
                assembly = AssemblyILWorker.LoadAssembly(dllBytes);
            }

            if (null != assembly)
            {
                Debug.Log(Log.Zero1("外部程序集执行方式：[Assembly]"));
                //使用Assembly                
                iLWorker = new AssemblyILWorker(assembly);
            }
            else
            {
                Debug.Log(Log.Zero1("外部程序集执行方式：[ILRuntime]"));
                //使用ILRuntime
                var ilruntimeWorker = new ILRuntimeILWorker(dllBytes, dllDir, dllName, isDebug, isNeedPdbFile);
                iLWorker = ilruntimeWorker;

                ILRuntimeAppDomain = ilruntimeWorker.appDomain;
                IsILRuntimeMode = true;
            }
        }

        public void Invoke(string clsName, string methodName)
        {
            iLWorker.Invoke(clsName, methodName);
        }

        private void OnGUI()
        {
            onGUI?.Invoke();
        }

        void Update()
        {
            onUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            onFixedUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            onLateUpdate?.Invoke();
        }

        private void OnApplicationFocus(bool focus)
        {
            onApplicationFocus?.Invoke(focus);
        }

        private void OnApplicationPause(bool pause)
        {
            onApplicationPause?.Invoke(pause);
        }

        private void OnApplicationQuit()
        {
            onApplicationQuit?.Invoke();
        }

        #region 协程代理
        Dictionary<object, CoroutineProxy> _routineDic = new Dictionary<object, CoroutineProxy>();

        CoroutineProxy GetCoroutineProxy(object target, bool isAutoCreate)
        {
            CoroutineProxy cp;
            _routineDic.TryGetValue(target, out cp);

            if (null == cp && isAutoCreate)
            {
                GameObject go = new GameObject("CoroutineProxy_" + target.GetHashCode());
                go.transform.SetParent(transform);
                cp = go.AddComponent<CoroutineProxy>();
                cp.bindingObj = target;
                cp.onDestroy += (proxy) =>
                {
                    _routineDic.Remove(proxy.bindingObj);
                };
                _routineDic[target] = cp;
            }

            return cp;
        }

        public Coroutine StartCoroutine(object target, IEnumerator coroutine)
        {
            var cp = GetCoroutineProxy(target, true);
            return cp.StartTrackedCoroutine(coroutine);
        }

        public void StopCoroutine(object target, IEnumerator routine)
        {
            var cp = GetCoroutineProxy(target, false);
            cp?.StopTrackedCoroutine(routine);
        }

        public void StopCoroutine(object target, Coroutine routine)
        {
            var cp = GetCoroutineProxy(target, false);
            cp?.StopTrackedCoroutine(routine);
        }

        public void StopAllCoroutines(object target)
        {
            var cp = GetCoroutineProxy(target, false);
            if (null != cp)
            {
                cp.StopAllTrackedCoroutines();
            }
        }
        #endregion
    }
}