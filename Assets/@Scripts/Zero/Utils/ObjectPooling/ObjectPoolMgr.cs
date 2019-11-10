using System.Collections.Generic;

namespace ZeroHot
{
    /// <summary>
    /// 对象池管理器
    /// </summary>
    class ObjectPoolMgr : ASingleton<ObjectPoolMgr>
    {
        Dictionary<string, BaseObjectPool> _poolDic = new Dictionary<string, BaseObjectPool>();

        public string[] GetPoolNames()
        {
            var keys = _poolDic.Keys;
            string[] names = new string[keys.Count];
            keys.CopyTo(names, 0);
            return names;
        }

        public ObjectPool<T> CreatePool<T>(string name, int poolMaxSize) where T : class, IRecyclable
        {
            if (_poolDic.ContainsKey(name))
            {
                throw new System.Exception(string.Format("已经有一个名为[{0}]的对象池了!", name));
            }

            var pool = new ObjectPool<T>(poolMaxSize);
            pool.name = name;
            _poolDic[name] = pool;
            return pool;
        }

        public ObjectPool<T> GetPool<T>(string name) where T : class, IRecyclable
        {
            if (_poolDic.ContainsKey(name))
            {
                var pool = _poolDic[name] as ObjectPool<T>;
                if (null == pool)
                {
                    throw new System.Exception(string.Format("对象池[{0}]存在，但是类型为:{1} , 并不是:{2} !", name, _poolDic[name].GetType().FullName, typeof(T).FullName));
                }
                return pool;
            }
            return null;
        }

        public void DestroyPool<T>(string name) where T : class, IRecyclable
        {
            if (_poolDic.ContainsKey(name))
            {
                var pool = _poolDic[name] as ObjectPool<T>;
                if (null == pool)
                {
                    throw new System.Exception(string.Format("对象池[{0}]存在，但是类型为:{1} , 并不是:{2} !", name, _poolDic[name].GetType().FullName, typeof(T).FullName));
                }
                pool.Clear();
                _poolDic.Remove(name);
            }
        }

        public override void Destroy()
        {
            foreach (var pool in _poolDic)
            {
                pool.Value.Clear();
            }
            _poolDic.Clear();
        }
    }
}
