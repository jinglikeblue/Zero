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
        public event Action<string> onCallback;

        public void Callback(string content)
        {
            if (null != onCallback)
            {
                onCallback.Invoke(content);
            }
        }
    }
}
