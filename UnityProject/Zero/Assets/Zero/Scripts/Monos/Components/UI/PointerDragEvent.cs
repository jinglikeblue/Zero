using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 按钮弹起的事件
    /// </summary>
    public class PointerDragEvent : MonoBehaviour,IDragHandler
    {
        public Action<PointerEventData> onEvent;

        public void OnDrag(PointerEventData eventData)
        {
            if (null == onEvent)
            {
                return;
            }
            onEvent.Invoke(eventData);
        }

        public static PointerDragEvent Get(GameObject gameObject)
        {
            return ComponentUtil.AutoGet<PointerDragEvent>(gameObject);
        }
    }
}