using System.Collections.Generic;

namespace ZeroHot
{
    public abstract class BaseObjectPool
    {
        public int poolMaxSize { get; protected set; }

        public string name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="poolMaxSize">线程池的大小，回收时如果池子大小已满，则多余的对象会被遗弃</param>
        public BaseObjectPool(int poolMaxSize)
        {
            this.poolMaxSize = poolMaxSize;
        }

        public abstract void Clear();
    };

    /// <summary>
    /// 对象池
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public class ObjectPool<TObject> : BaseObjectPool where TObject : class, IRecyclable
    {
        List<TObject> _poolList = new List<TObject>();

        public int CurrentSize
        {
            get
            {
                return _poolList.Count;
            }
        }

        public ObjectPool(int poolMaxSize) : base(poolMaxSize)
        {
        }

        /// <summary>
        /// 改变对象池大小
        /// </summary>
        /// <param name="poolMaxSize"></param>
        public void ChangeSize(int poolMaxSize)
        {
            this.poolMaxSize = poolMaxSize;
            if(_poolList.Count > poolMaxSize)
            {
                for(int i = _poolList.Count - 1; i >= poolMaxSize; i--)
                {
                    _poolList[i].Discarded();
                    _poolList.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <returns></returns>
        public TObject GetInstance()
        {
            while (_poolList.Count > 0)
            {
                var ins = _poolList[0];
                _poolList.RemoveAt(0);
                return ins;
            }
            return default;
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj"></param>
        public void Recycle(TObject obj)
        {
            if (_poolList.Count < poolMaxSize)
            {
                obj.Recycled();
                _poolList.Add(obj);
            }
            else
            {
                obj.Discarded();
            }
        }

        /// <summary>
        /// 是否在对象池中
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool IsInPool(TObject obj)
        {
            return _poolList.IndexOf(obj) > -1 ? true : false;
        }

        /// <summary>
        /// 清空对象池
        /// </summary>
        public override void Clear()
        {
            foreach (var obj in _poolList)
            {
                obj.Discarded();
            }
            _poolList.Clear();
        }
    }
}
