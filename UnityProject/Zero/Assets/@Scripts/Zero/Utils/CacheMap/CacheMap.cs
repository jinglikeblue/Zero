using System.Collections.Generic;

namespace ZeroHot
{
    /// <summary>
    /// 缓存表工具
    /// </summary>
    class CacheMap<TKey,TValue>
    {
        public readonly Dictionary<TKey, TValue> cache;

        public CacheMap()
        {
            cache = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public void Set(TKey key, TValue obj)
        {
            cache[key] = obj;
        }

        /// <summary>
        /// 是否包含目标缓存
        /// </summary>
        /// <param name="key"></param>
        public bool Contines(TKey key)
        {
            return cache.ContainsKey(key);
        }

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Get(TKey key, out TValue value)
        {
            if (cache.ContainsKey(key))
            {
                value = cache[key];
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 获取以指定类型获取缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Get<T>(TKey key, out T value)
        {
            if (cache.ContainsKey(key))
            {
                object obj = cache[key];
                value = (T)obj;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key"></param>        
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            if (cache.ContainsKey(key))
            {
                cache.Remove(key);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        public void Clear()
        {
            cache.Clear();
        }
    }
}
