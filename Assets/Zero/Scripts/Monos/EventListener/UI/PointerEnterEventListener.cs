using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 按钮弹起的事件
    /// </summary>
    public class PointerEnterEventListener : AEventListener<PointerEnterEventListener>, IPointerEnterHandler
    {
        public event Action<PointerEventData> onEvent;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (null == onEvent)
            {
                return;
            }
            onEvent.Invoke(eventData);
        }

        public static PointerEnterEventListener Get(GameObject gameObject)
        {
            return ComponentUtil.AutoGet<PointerEnterEventListener>(gameObject);
        }
    }
}