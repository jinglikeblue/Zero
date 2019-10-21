using System;
using System.Reflection;
using UnityEngine;

namespace Zero
{
    public class AssemblyILWorker : BaseILWorker
    {
        /// <summary>
        /// 从指定二进制数据加载Assembly，失败则返回null
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Assembly LoadAssembly(byte[] assemblyBytes)
        {
            Assembly assembly;
            try
            {
                //反射执行
                assembly = Assembly.Load(assemblyBytes);
            }
            catch
            {
                assembly = null;
            }
            return assembly;
        }

        Assembly _assembly = null;

        public AssemblyILWorker(Assembly assembly)
        {
            Debug.Log(Log.Zero1("外部程序集执行方式：[Assembly]"));
            _assembly = assembly;
        }

        public override void Invoke(string clsName, string methodName)
        {
            if (null != _assembly)
            {
                //反射执行                
                Type type = _assembly.GetType(clsName);
                MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
                method.Invoke(null, null);                
            }
        }
    }
}