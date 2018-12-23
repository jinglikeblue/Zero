using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IL.Zero
{
    /// <summary>
    /// 单例模式基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ASingleton<T> where T:new()
    {
        private static T _ins = new T();

        public static T Ins
        {
            get { return _ins; }
        }
    }
}
