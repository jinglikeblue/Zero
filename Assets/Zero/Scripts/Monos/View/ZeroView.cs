using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Zero
{
    public class ZeroView : MonoBehaviour
    {
        /// <summary>
        /// 是否已执行Awake
        /// </summary>
        public bool IsAwake { get; private set; } = false;

        /// <summary>
        /// 是否已执行Start
        /// </summary>
        public bool IsStart { get; private set; } = false;

        public event Action onEnable;
        public event Action onDisable;
        public event Action onDestroy;

        [Header("AView子类名称")]
        public string aViewClass;

        private void Awake()
        {
            IsAwake = true;
        }

        private void Start()
        {
            IsStart = true;
        }

        private void OnEnable()
        {
            onEnable?.Invoke();
        }

        private void OnDisable()
        {
            onDisable?.Invoke();
        }

        private void OnDestroy()
        {
            onDestroy?.Invoke();

            //清空没用的事件
            onEnable = null;
            onDisable = null;
            onDestroy = null;
        }
    }
}