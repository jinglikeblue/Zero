using System;
using System.Collections.Generic;
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

        public Assembly assembly { get; private set; }

        public AssemblyILWorker(Assembly assembly)
        {            
            this.assembly = assembly;
        }

        public override void Invoke(string clsName, string methodName)
        {
            if (null != assembly)
            {
                //反射执行                
                Type type = assembly.GetType(clsName);
                MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
                method.Invoke(null, null);                
            }
        }

        public override Type[] GetTypes(Func<Type, bool> whereFunc = null)
        {
            var types = assembly.GetTypes();

            if (null != whereFunc)
            {
                var typeList = new List<Type>();
                foreach (var type in types)
                {
                    if (whereFunc.Invoke(type))
                    {
                        typeList.Add(type);
                    }
                }

                types = typeList.ToArray();
            }

            return types;
        }
    }
}