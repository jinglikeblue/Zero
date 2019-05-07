using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 该组件实现了UI常用的事件接口，如果要使用更高效的UI事件组件，请使用针对于具体事件的PointerClickEvent等组件。<para></para>
    /// PS：没有实现onDrag事件，因为该事件会与ScrollView组件冲突，请使用onMove替代。
    /// </summary>
    public class UIEventListener : AEventListener<UIEventListener>,
        IPointerClickHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        /// <summary>
        /// 点击事件
        /// </summary>
        public event Action<PointerEventData> onClick;
        /// <summary>
        /// 目标按下事件
        /// </summary>
        public event Action<PointerEventData> onPointerDown;
        /// <summary>
        /// 目标抬起事件
        /// </summary>
        public event Action<PointerEventData> onPointerUp;
        /// <summary>
        /// 目标进入事件
        /// </summary>
        public event Action<PointerEventData> onPointerEnter;
        /// <summary>
        /// 目标退出事件
        /// </summary>
        public event Action<PointerEventData> onPointerExit;

        /// <summary>
        /// 获取GameObject的UI事件组件，没有则会自动添加
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static UIEventListener Get(GameObject gameObject)
        {
            return ComponentUtil.AutoGet<UIEventListener>(gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (null != onClick)
            {
                onClick.Invoke(eventData);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (null != onPointerDown)
            {
                onPointerDown.Invoke(eventData);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (null != onPointerUp)
            {
                onPointerUp.Invoke(eventData);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (null != onPointerEnter)
            {
                onPointerEnter.Invoke(eventData);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (null != onPointerExit)
            {
                onPointerExit.Invoke(eventData);
            }
        }
    }
}
