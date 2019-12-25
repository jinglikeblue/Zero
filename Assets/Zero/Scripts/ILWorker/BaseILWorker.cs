using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero
{
    public abstract class BaseILWorker
    {
        /// <summary>
        /// 调用方法
        /// </summary>
        /// <param name="clsName"></param>
        /// <param name="methodName"></param>
        public abstract void Invoke(string clsName, string methodName);

        /// <summary>
        /// 获取类型清单
        /// </summary>
        /// <param name="whereFunc">可选参数，委托通过参数Type判断是否需要加入清单中，返回true则表示需要</param>
        /// <returns></returns>
        public abstract Type[] GetTypes(Func<Type, bool> whereFunc = null);
    }
}