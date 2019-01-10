using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 按钮弹起的事件
    /// </summary>
    public class PointerExitEventListener : AEventListener<PointerExitEventListener>, IPointerExitHandler
    {
        public event Action<PointerEventData> onEvent;

        public void OnPointerExit(PointerEventData eventData)
        {
            if (null == onEvent)
            {
                return;
            }
            onEvent.Invoke(eventData);
        }

        public static PointerExitEventListener Get(GameObject gameObject)
        {
            return ComponentUtil.AutoGet<PointerExitEventListener>(gameObject);
        }
    }
}