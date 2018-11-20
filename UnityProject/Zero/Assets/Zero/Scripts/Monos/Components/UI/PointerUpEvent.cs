using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 按钮弹起的事件
    /// </summary>
    public class PointerUpEvent : MonoBehaviour,IPointerUpHandler
    {
        public Action<PointerEventData> onEvent;

        public void OnPointerUp(PointerEventData eventData)
        {
            if (null == onEvent)
            {
                return;
            }
            onEvent.Invoke(eventData);
        }

        public static PointerUpEvent Get(GameObject gameObject)
        {
            return ComponentUtil.AutoGet<PointerUpEvent>(gameObject);
        }
    }
}