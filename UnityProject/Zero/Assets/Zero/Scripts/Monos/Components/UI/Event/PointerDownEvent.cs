using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 按钮按下的事件
    /// </summary>
    public class PointerDownEvent : MonoBehaviour,IPointerDownHandler
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

        public static PointerDownEvent Get(GameObject gameObject)
        {
            return ComponentUtil.AutoGet<PointerDownEvent>(gameObject);
        }
    }
}