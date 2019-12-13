namespace ZeroHot
{
    public abstract class BaseSingleton
    {
        /// <summary>
        /// 初始化方法
        /// </summary>
        protected abstract void Init();

        /// <summary>
        /// 执行销毁
        /// </summary>
        public abstract void Destroy();
    };

    /// <summary>
    /// 单例模式基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ASingleton<T> : BaseSingleton where T : BaseSingleton, new()
    {
        private static object _singletonLock = new object(); //锁同步

        private static T _ins;

        /// <summary>
        /// 单例
        /// </summary>
        public static T Ins
        {
            get
            {                
                return GetIns();
            }
        }

        /// <summary>
        /// 创建单例，如果单例对象不存在，则会创建
        /// </summary>
        public static T GetIns()
        {
            lock (_singletonLock)
            {
                if (null == _ins)
                {
                    _ins = new T();
                }
            }
            return _ins;
        }

        /// <summary>
        /// 重置单例，下次获取单例时会创建新的实例对象
        /// </summary>
        public static void ResetIns()
        {
            _ins?.Destroy();
            _ins = default(T);
        }

        protected ASingleton()
        {
            Init();
        }
    }
}
