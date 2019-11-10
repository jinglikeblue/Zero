using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 按钮弹起的事件
    /// </summary>
    public class PointerClickEventListener : AEventListener<PointerClickEventListener>, IPointerClickHandler
    {
        public event Action<PointerEventData> onEvent;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (null == onEvent)
            {
                return;
            }
            onEvent.Invoke(eventData);
        }

        public static PointerClickEventListener Get(GameObject gameObject)
        {
            return ComponentUtil.AutoGet<PointerClickEventListener>(gameObject);
        }
    }
}