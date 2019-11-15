using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroHot;

/// <summary>
/// 携程代理工具
/// </summary>
public class CoroutineProxy : MonoBehaviour
{
    /// <summary>
    /// 绑定的对象
    /// </summary>
    public object bindingObj;

    public event Action<CoroutineProxy> onDestroy;

    HashSet<TrackedCoroutine> _tcSet = new HashSet<TrackedCoroutine>();

    public Coroutine StartTrackedCoroutine(IEnumerator routine)
    {        
        var tc = new TrackedCoroutine(routine);
        _tcSet.Add(tc);
        tc.onComplete += RemoveTC;
        tc.coroutine = base.StartCoroutine(tc);
        return tc.coroutine;
    }

    private void RemoveTC(TrackedCoroutine tc)
    {
        tc.onComplete -= RemoveTC;
        _tcSet.Remove(tc);
        DestroyIfEmpty();
    }

    public void StopTrackedCoroutine(Coroutine routine)
    {
        TrackedCoroutine tc = null;
        foreach (var temp in _tcSet)
        {
            if(temp.coroutine == routine)
            {
                tc = temp;
                break;
            }
        }

        if (tc != null)
        {
            RemoveTC(tc);
            base.StopCoroutine(routine);
        }
    }

    public void StopTrackedCoroutine(IEnumerator routine)
    {
        TrackedCoroutine tc = null;
        foreach (var temp in _tcSet)
        {
            if (temp.trackedCoroutine == routine)
            {
                tc = temp;
                break;
            }
        }

        if (tc != null)
        {
            RemoveTC(tc);
            base.StopCoroutine(routine);
        }
    }

    public void StopAllTrackedCoroutines()
    {
        base.StopAllCoroutines();
        _tcSet.Clear();
        DestroyIfEmpty();
    }

    /// <summary>
    /// 是否没有任何协程在执行
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        return _tcSet.Count == 0 ? true : false;
    }

    public void DestroyIfEmpty()
    {
        if (_tcSet.Count == 0)
        {
            GameObject.DestroyImmediate(this.gameObject);            
        }
    }

    void OnDestroy()
    {
        onDestroy?.Invoke(this);
    }

    public class TrackedCoroutine : IEnumerator
    {
        /// <summary>
        /// 协程执行完毕
        /// </summary>
        public event Action<TrackedCoroutine> onComplete;        

        /// <summary>
        /// 追踪的携程
        /// </summary>
        public readonly IEnumerator trackedCoroutine;

        /// <summary>
        /// 关联的Coroutine对象
        /// </summary>
        public Coroutine coroutine;

        public TrackedCoroutine(IEnumerator routine)
        {
            trackedCoroutine = routine;

            // 在这里标记协程的创建
        }

        object IEnumerator.Current
        {
            get
            {
                return trackedCoroutine.Current;
            }
        }

        public bool MoveNext()
        {
            // 在这里可以：
            //     1. 标记协程的执行
            //     2. 记录协程本次执行的时间

            bool next = trackedCoroutine.MoveNext();

            if (next)
            {
                // 一次普通的执行
            }
            else
            {
                // 协程运行到末尾，已结束
                onComplete?.Invoke(this);
            }

            return next;
        }

        public void Reset()
        {
            trackedCoroutine.Reset();
        }
    }
}
