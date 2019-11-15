using System;
using System.Collections;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 计时器
    /// </summary>
    public class Timer
    {
        public enum EState
        {
            /// <summary>
            /// 空闲
            /// </summary>
            IDLE,

            /// <summary>
            /// 执行中
            /// </summary>
            RUNNING,

            /// <summary>
            /// 暂停中
            /// </summary>
            PAUSING,
        }

        /// <summary>
        /// 间隔时间
        /// </summary>
        public float intervalSeconds { get; private set; }

        /// <summary>
        /// 本次执行设定的总触发次数
        /// </summary>
        public uint repeatCount { get; private set; }

        /// <summary>
        /// 已出发次数
        /// </summary>
        public int triggeredTimes { get; private set; }

        /// <summary>
        /// 计时器当前状态
        /// </summary>
        public EState state { get; private set; } = EState.IDLE;

        /// <summary>
        /// 触发了计时器事件
        /// </summary>
        public event Action<Timer> onTriggered;

        /// <summary>
        /// 下次触发的时间标记
        /// </summary>
        DateTime _triggerDT;

        /// <summary>
        /// 暂停的时间标记
        /// </summary>
        DateTime _stopDT;

        /// <summary>
        /// 距离下次触发剩余的时间
        /// </summary>
        public float RemainingSeconds
        {
            get
            {
                float result = 0;
                switch (state)
                {
                    case EState.RUNNING:
                        {
                            var tn = _triggerDT - DateTime.UtcNow;
                            result = (float)tn.TotalSeconds;
                        }
                        break;
                    case EState.PAUSING:
                        {
                            var tn = _triggerDT - _stopDT;
                            result = (float)tn.TotalSeconds;
                        }
                        break;
                }
                return result;
            }
        }

        /// <summary>
        /// 定时器
        /// </summary>
        /// <param name="intervalSeconds">间隔时间</param>
        public Timer(float intervalSeconds)
        {
            Reset(intervalSeconds);
        }

        /// <summary>
        /// 清理事件的所有监听
        /// </summary>
        public void ClearEventListeners()
        {
            onTriggered = null;
        }

        /// <summary>
        /// 重置计时器
        /// </summary>
        /// <param name="intervalSeconds">新的间隔时间</param>
        public void Reset(float intervalSeconds)
        {            
            Reset();            
            this.intervalSeconds = intervalSeconds;
        }

        void Reset()
        {
            Stop();
            triggeredTimes = 0;            
        }

        /// <summary>
        /// 重新启动计时器
        /// </summary>
        /// <param name="repeatCount">重复次数，如果为0则无限次重复。否则只重复指定数量次数</param>
        public void Start(uint repeatCount = 0)
        {
            Reset();
            this.repeatCount = repeatCount;
            _triggerDT = DateTime.UtcNow.AddSeconds(intervalSeconds);
            ILBridge.Ins.StartCoroutine(this, Update());
        }        

        /// <summary>
        /// 暂停计时器，
        /// </summary>
        public void Pause()
        {
            if (state == EState.RUNNING)
            {                
                _stopDT = DateTime.UtcNow;
                state = EState.PAUSING;
                ILBridge.Ins.StopAllCoroutines(this);
            }
        }

        /// <summary>
        /// 如果计时器暂停了，让其继续执行
        /// </summary>
        public void Continue()
        {
            if(state == EState.PAUSING)
            {
                _triggerDT = DateTime.UtcNow.AddSeconds(RemainingSeconds);                
                ILBridge.Ins.StartCoroutine(this, Update());                
            }
        }

        IEnumerator Update()
        {
            state = EState.RUNNING;
            while (true)
            {
                //计算到下次触发要等待的间隔    
                var tn = _triggerDT - DateTime.UtcNow;                
                yield return new WaitForSeconds((float)tn.TotalSeconds);
                triggeredTimes++;
                onTriggered?.Invoke(this);
                if(triggeredTimes == repeatCount)
                {
                    //停止计时器
                    Stop();
                }
                else
                {
                    //计算下一次触发时间
                    _triggerDT = DateTime.UtcNow.AddSeconds(intervalSeconds);
                }
            }
        }

        /// <summary>
        /// 停止计时器
        /// </summary>
        public void Stop()
        {
            _triggerDT = DateTime.MaxValue;
            state = EState.IDLE;
            ILBridge.Ins.StopAllCoroutines(this);
        }
    }
}


