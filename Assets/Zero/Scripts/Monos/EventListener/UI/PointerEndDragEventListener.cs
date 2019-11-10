using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 按钮弹起的事件
    /// </summary>
    public class PointerEndDragEventListener : AEventListener<PointerEndDragEventListener>, IEndDragHandler
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

        public static PointerEndDragEventListener Get(GameObject gameObject)
        {
            return ComponentUtil.AutoGet<PointerEndDragEventListener>(gameObject);
        }
    }
}