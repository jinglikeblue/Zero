using System;
using UnityEngine;
using UnityEngine.Events;

namespace Zero
{
    /// <summary>
    /// 动画回调
    /// </summary>
    public class AnimationCallback : MonoBehaviour
    {
        public event Action<string> onCallbak;

        public void Callback(string content)
        {
            onCallbak?.Invoke(content);
        }
    }
}
