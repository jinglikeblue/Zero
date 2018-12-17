using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    public class ILRuntimeBridge : MonoBehaviour
    {
        const string BRIDGE_NAME = "ILRuntimeBridge";

        static ILRuntimeBridge _ins;

        public static ILRuntimeBridge Ins
        {
            get
            {
                if (null == _ins)
                {
                    GameObject ins = GameObject.Find(BRIDGE_NAME);
                    if (null == ins)
                    {
                        ins = new GameObject();
                    }
                    ins.name = BRIDGE_NAME;
                    _ins = ins.AddComponent<ILRuntimeBridge>();
                    GameObject.DontDestroyOnLoad(ins);
                }

                return _ins;
            }
        }

        public string libDir;
        public string libName;

        //AppDomain是ILRuntime的入口，最好是在一个单例类中保存，整个游戏全局就一个，这里为了示例方便，每个例子里面都单独做了一个
        //大家在正式项目中请全局只创建一个AppDomain
        ILRuntime.Runtime.Enviorment.AppDomain _appdomain = null;

        /// <summary>
        /// Update事件委托
        /// </summary>
        public System.Action onUpdate;

        /// <summary>
        /// OnGUI事件委托
        /// </summary>
        public System.Action onGUI;

        /// <summary>
        /// OnFixedUpdate事件委托
        /// </summary>
        public System.Action onFixedUpdate;

        /// <summary>
        /// 启动ILRuntime
        /// </summary>
        /// <param name="libDir">库的目录</param>
        /// <param name="libName">库的名称</param>
        /// <param name="clsName">ILRuntime的启动类</param>
        /// <param name="methodName">ILRuntime的启动方法</param>
        public void Startup(string libDir, string libName, bool isDebug, bool isNeedPdbFile)
        {
            this.libDir = libDir;
            this.libName = libName;     

            //首先实例化ILRuntime的AppDomain，AppDomain是一个应用程序域，每个AppDomain都是一个独立的沙盒
            _appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
            
            if (isDebug)
            {
                //启动调试监听
                _appdomain.DebugService.StartDebugService(56000);
            }

            string dllPath = Path.Combine(libDir, libName + ".dll");
            byte[] dllBytes = File.ReadAllBytes(dllPath);

            if (isNeedPdbFile)
            {
                string pdbPath = Path.Combine(libDir, libName + ".pdb");
                byte[] pdbBytes = File.ReadAllBytes(pdbPath);
                using (System.IO.MemoryStream fs = new MemoryStream(dllBytes))
                {
                    using (System.IO.MemoryStream p = new MemoryStream(pdbBytes))
                    {
                        _appdomain.LoadAssembly(fs, p, new Mono.Cecil.Pdb.PdbReaderProvider());                        
                    }
                }
            }
            else
            {
                using (System.IO.MemoryStream fs = new MemoryStream(dllBytes))
                {
                    _appdomain.LoadAssembly(fs);
                }
            }



            InitializeILRuntime();
        }

        void InitializeILRuntime()
        {
            var appdomain = _appdomain;

            //注册LitJson
            LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(appdomain);

            //注册CLR绑定
            ILRuntime.Runtime.Generated.CLRBindings.Initialize(appdomain);

            //使用Couroutine时，C#编译器会自动生成一个实现了IEnumerator，IEnumerator<object>，IDisposable接口的类，因为这是跨域继承，所以需要写CrossBindAdapter
            appdomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
            //appdomain.RegisterCrossBindingAdaptor(new WebClientAdapter());

            //这里做一些ILRuntime的注册

            #region Action泛型转换
            appdomain.DelegateManager.RegisterMethodDelegate<float>();
            appdomain.DelegateManager.RegisterMethodDelegate<PointerEventData>();
            appdomain.DelegateManager.RegisterMethodDelegate<AxisEventData>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Object>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Collider2D>();
            #endregion

            appdomain.DelegateManager.RegisterMethodDelegate<System.Object, System.Net.DownloadProgressChangedEventArgs>();
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Net.DownloadProgressChangedEventHandler>((act) =>
            {
                return new System.Net.DownloadProgressChangedEventHandler((sender, e) =>
                {
                    ((Action<System.Object, System.Net.DownloadProgressChangedEventArgs>)act)(sender, e);
                });
            });

            appdomain.DelegateManager.RegisterMethodDelegate<System.Object, System.ComponentModel.AsyncCompletedEventArgs>();
            appdomain.DelegateManager.RegisterDelegateConvertor<System.ComponentModel.AsyncCompletedEventHandler>((act) =>
            {
                return new System.ComponentModel.AsyncCompletedEventHandler((sender, e) =>
                {
                    ((Action<System.Object, System.ComponentModel.AsyncCompletedEventArgs>)act)(sender, e);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Threading.ThreadStart>((act) =>
            {
                return new System.Threading.ThreadStart(() =>
                {
                    ((Action)act)();
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
            {
                return new UnityEngine.Events.UnityAction(() =>
                {
                    ((Action)act)();
                });
            });
        }

        public void Invoke(string clsName, string methodName)
        {
            _appdomain.Invoke(clsName, methodName, null, null);            
        }

        public UnityEngine.Object GetAsset(string abName, string assetName)
        {
            return ResMgr.Ins.Load<UnityEngine.Object>(abName, assetName);
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
    }
}