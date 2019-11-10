using System;
using System.Collections.Generic;

namespace ZeroHot
{
    /// <summary>
    /// 在线程A中通过AddToSyncAction将Action添加进来，在线程B中通过RunSyncActions来执行这些方法。
    /// </summary>
    public class ThreadSyncActions
    {
        List<Action> _toSyncActinList = new List<Action>();

        /// <summary>
        /// 添加要同步到执行线程的Action
        /// </summary>
        /// <param name="action"></param>
        public void AddToSyncAction(Action action)
        {
            lock (_toSyncActinList)
            {
                _toSyncActinList.Add(action);
            }
        }

        /// <summary>
        /// 清空所有要同步的Action
        /// </summary>
        public void Clear()
        {
            lock (_toSyncActinList)
            {
                _toSyncActinList.Clear();
            }
        }

        /// <summary>
        /// 在执行现场调用该方法，执行所有要同步的Action
        /// </summary>
        public void RunSyncActions()
        {
            if (_toSyncActinList.Count > 0)
            {
                List<Action> actionCacheList = null;

                lock (_toSyncActinList)
                {
                    actionCacheList = _toSyncActinList.GetRange(0, _toSyncActinList.Count);
                    _toSyncActinList.Clear();
                }

                for (int i = 0; i < actionCacheList.Count; i++)
                {
                    actionCacheList[i].Invoke();
                }
            }
        }
    }
}
