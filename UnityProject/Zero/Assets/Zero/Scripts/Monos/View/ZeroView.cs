using System;
using UnityEngine;

namespace Zero
{
    public class ZeroView : MonoBehaviour
    {        
        /// <summary>
        /// 是否已执行Start
        /// </summary>
        public bool IsStart { get; protected set; }

        public event Action onEnable;
        public event Action onDisable;
        public event Action onDestroy;

        private void Start()
        {
            IsStart = true;
        }

        private void OnEnable()
        {
            if(null != onEnable)
            {
                onEnable.Invoke();
            }            
        }

        private void OnDisable()
        {
            if(null != onDisable)
            {
                onDisable.Invoke();
            }            
        }

        private void OnDestroy()
        {
            if(null != onDestroy)
            {
                onDestroy.Invoke();
            }            

            //清空没用的事件
            onEnable = null;
            onDisable = null;
            onDestroy = null;
        }        
    }
}