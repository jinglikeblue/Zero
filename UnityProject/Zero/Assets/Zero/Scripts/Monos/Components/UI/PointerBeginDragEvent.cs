using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 按钮弹起的事件
    /// </summary>
    public class PointerBeginDragEvent : MonoBehaviour,IBeginDragHandler
    {
        public Action<PointerEventData> onEvent;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(null == onEvent)
            {
                return;
            }
            onEvent.Invoke(eventData);
        }

        public static PointerBeginDragEvent Get(GameObject gameObject)
        {
            return ComponentUtil.AutoGet<PointerBeginDragEvent>(gameObject);
        }
    }
}