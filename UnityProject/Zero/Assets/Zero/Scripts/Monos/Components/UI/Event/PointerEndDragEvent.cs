using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 按钮弹起的事件
    /// </summary>
    public class PointerEndDragEvent : MonoBehaviour,IEndDragHandler
    {
        public event Action<PointerEventData> onEvent;

        public void OnEndDrag(PointerEventData eventData)
        {
            if (null == onEvent)
            {
                return;
            }
            onEvent.Invoke(eventData);
        }

        public static PointerEndDragEvent Get(GameObject gameObject)
        {
            return ComponentUtil.AutoGet<PointerEndDragEvent>(gameObject);
        }
    }
}