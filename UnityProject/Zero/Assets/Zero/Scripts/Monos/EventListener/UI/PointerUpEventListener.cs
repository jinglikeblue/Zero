using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 按钮弹起的事件
    /// </summary>
    public class PointerUpEventListener : AEventListener<PointerUpEventListener>, IPointerUpHandler
    {
        public event Action<PointerEventData> onEvent;

        public void OnPointerUp(PointerEventData eventData)
        {
            if (null == onEvent)
            {
                return;
            }
            onEvent.Invoke(eventData);
        }

        public static PointerUpEventListener Get(GameObject gameObject)
        {
            return ComponentUtil.AutoGet<PointerUpEventListener>(gameObject);
        }
    }
}