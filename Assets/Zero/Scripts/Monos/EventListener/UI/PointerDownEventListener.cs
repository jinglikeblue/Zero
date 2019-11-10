using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 按钮按下的事件
    /// </summary>
    public class PointerDownEventListener : AEventListener<PointerDownEventListener>, IPointerDownHandler
    {
        public event Action<PointerEventData> onEvent;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (null == onEvent)
            {
                return;
            }
            onEvent.Invoke(eventData);
        }

        public static PointerDownEventListener Get(GameObject gameObject)
        {
            return ComponentUtil.AutoGet<PointerDownEventListener>(gameObject);
        }
    }
}