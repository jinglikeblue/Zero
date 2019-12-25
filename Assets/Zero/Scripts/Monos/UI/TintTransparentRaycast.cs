using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Zero
{
    /// <summary>
    /// 该组件在TransparentRaycast的基础上，添加可以在按下时对指定图形染色的功能
    /// </summary>
    public class TintTransparentRaycast : TransparentRaycast
    {
        [Serializable]
        public class TintGraphic
        {
            [OnValueChanged("OnTargetChange")]
            public Graphic target;            
            public Color pressedColor = Color.white;

            public Color normalColor { get; private set; }

            void OnTargetChange()
            {
                if (null != target)
                {                    
                    pressedColor = target.color;
                }
            }

            public void ResetNormalColor()
            {
                normalColor = target.color;
            }

            public void SetPressed()
            {
                target.color = pressedColor;
            }

            public void SetNormal()
            {
                target.color = normalColor;
            }
        }

        [TableList]
        public TintGraphic[] graphicsList;

        private void OnPointerUp(PointerEventData obj)
        {
            foreach (var graphic in graphicsList)
            {
                graphic.SetNormal();
            }
        }

        private void OnPointerDown(PointerEventData obj)
        {
            foreach (var graphic in graphicsList)
            {
                graphic.SetPressed();
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                foreach (var graphic in graphicsList)
                {
                    graphic.ResetNormalColor();
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (Application.isPlaying) //该类在非Editor模式下，也会执行OnEnable
            {
                if (null != graphicsList && graphicsList.Length > 0)
                {
                    PointerDownEventListener.Get(gameObject).onEvent += OnPointerDown;
                    PointerUpEventListener.Get(gameObject).onEvent += OnPointerUp;
                }
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (Application.isPlaying) //该类在非Editor模式下，也会执行OnEnable
            {
                if (null != graphicsList && graphicsList.Length > 0)
                {
                    PointerDownEventListener.Get(gameObject).onEvent -= OnPointerDown;
                    PointerUpEventListener.Get(gameObject).onEvent -= OnPointerUp;
                }
            }
        }
    }
    }
