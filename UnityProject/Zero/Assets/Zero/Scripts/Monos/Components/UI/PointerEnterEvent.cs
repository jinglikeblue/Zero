using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 按钮弹起的事件
    /// </summary>
    public class PointerEnterEvent : MonoBehaviour,IPointerEnterHandler
    {
        public Action<PointerEventData> onEvent;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (null == onEvent)
            {
                return;
            }
            onEvent.Invoke(eventData);
        }

        public static PointerEnterEvent Get(GameObject gameObject)
        {
            return ComponentUtil.AutoGet<PointerEnterEvent>(gameObject);
        }
    }
}