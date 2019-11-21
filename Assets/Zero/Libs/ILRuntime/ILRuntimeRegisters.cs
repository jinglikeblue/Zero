using System;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// ILRuntime的泛型适配器、重定向等功能注册类。
    /// 在使用泛型委托时，ILRuntime可能会因为发现热更代码向主工程注册的委托因为没有适配器而报错。
    /// 报错代码类似：KeyNotFoundException: Cannot find Delegate Adapter for:[XXX], Please add following code:
    /// 这时只需要将报错内容提示的代码(following code下面一行)粘贴到该类的Register方法中即可。
    /// </summary>
    public sealed class ILRuntimeRegisters
    {
        public ILRuntime.Runtime.Enviorment.AppDomain appdomain { get; }

        public ILRuntimeRegisters(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
        {
            this.appdomain = appdomain;
        }

        public void Register()
        {
            RegisterAdaptor();
            RegisterCLRRedirection();
            RegisterFunctionDelegate();
            RegisterDelegateConvertor();            
        }

        /// <summary>
        /// 注册适配器
        /// </summary>
        void RegisterAdaptor()
        {
            #region Zero框架使用
            //使用Couroutine时，C#编译器会自动生成一个实现了IEnumerator，IEnumerator<object>，IDisposable接口的类，因为这是跨域继承，所以需要写CrossBindAdapter
            appdomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
            #endregion
        }

        /// <summary>
        /// 注册重定向
        /// </summary>
        void RegisterCLRRedirection()
        {
            #region Zero框架使用
            //注册LitJson
            LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(appdomain);
            #endregion
        }

        /// <summary>
        /// 注册方法委托
        /// </summary>
        void RegisterFunctionDelegate()
        {
            #region Zero框架使用
            appdomain.DelegateManager.RegisterMethodDelegate<float>();
            appdomain.DelegateManager.RegisterMethodDelegate<PointerEventData>();
            appdomain.DelegateManager.RegisterMethodDelegate<AxisEventData>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Object>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Collider2D>();            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.String, System.String>();            appdomain.DelegateManager.RegisterMethodDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance>();            appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Object, System.Net.DownloadProgressChangedEventArgs>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Object, System.ComponentModel.AsyncCompletedEventArgs>();
            appdomain.DelegateManager.RegisterMethodDelegate<Zero.Timer>();
            #endregion            
        }

        /// <summary>
        /// 注册方法转换
        /// </summary>
        void RegisterDelegateConvertor()
        {
            #region Zero框架使用
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Net.DownloadProgressChangedEventHandler>((act) =>
            {
                return new System.Net.DownloadProgressChangedEventHandler((sender, e) =>
                {
                    ((Action<System.Object, System.Net.DownloadProgressChangedEventArgs>)act)(sender, e);
                });
            });
            
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

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
            {
                return new System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>((x, y) =>
                {
                    return ((Func<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>)act)(x, y);
                });
            });
            #endregion
        }
    }
}