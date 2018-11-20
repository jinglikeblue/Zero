using System;
using System.Net;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class WebClientAdapter : CrossBindingAdaptor
{
    public override Type BaseCLRType
    {
        get
        {
            return typeof(WebClient);
        }
    }

    public override Type[] BaseCLRTypes
    {
        get
        {
            return null;
        }
    }

    public override Type AdaptorType
    {
        get
        {
            return typeof(Adaptor);
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);//创建一个新的实例
    }

    class Adaptor : WebClient, CrossBindingAdaptorType
    {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        public ILTypeInstance ILInstance
        {
            get
            {
                return instance;
            }
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            IMethod m = appdomain.ObjectType.GetMethod("GetWebRequest", 1);
            m = instance.Type.GetVirtualMethod(m);
            if (m != null )
            {
                return (WebRequest)appdomain.Invoke(m, instance, address);
            }
            else
                return base.GetWebRequest(address);
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            return base.GetWebResponse(request);
        }
    }

}
