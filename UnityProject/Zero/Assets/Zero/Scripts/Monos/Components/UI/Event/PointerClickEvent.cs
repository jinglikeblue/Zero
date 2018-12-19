using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 按钮弹起的事件
    /// </summary>
    public class PointerClickEvent : MonoBehaviour,IPointerClickHandler
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

        public static PointerClickEvent Get(GameObject gameObject)
        {
            return ComponentUtil.AutoGet<PointerClickEvent>(gameObject);
        }
    }
}