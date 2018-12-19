using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 按钮弹起的事件
    /// </summary>
    public class PointerMoveEvent : MonoBehaviour,IMoveHandler
    {
        public event Action<AxisEventData> onEvent;

        public void OnMove(AxisEventData eventData)
        {
            if (null == onEvent)
            {
                return;
            }
            onEvent.Invoke(eventData);
        }

        public static PointerMoveEvent Get(GameObject gameObject)
        {
            return ComponentUtil.AutoGet<PointerMoveEvent>(gameObject);
        }
    }
}