namespace Zero
{
    /// <summary>
    /// ILRuntime的泛型适配器注册类。
    /// 在使用泛型委托时，ILRuntime可能会因为发现热更代码向主工程注册的委托因为没有适配器而报错。
    /// 报错代码类似：KeyNotFoundException: Cannot find Delegate Adapter for:[XXX], Please add following code:
    /// 这时只需要将报错内容提示的代码(following code下面一行)粘贴到该类的Register方法中即可。
    /// </summary>
    public abstract class BaseILRuntimeGenerics 
    {
        public BaseILRuntimeGenerics()
        {

        }

        public abstract void Register(ILRuntime.Runtime.Enviorment.AppDomain appdomain);
    }
}