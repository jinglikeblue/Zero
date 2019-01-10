using UnityEngine;
using UnityEngine.UI;

namespace Zero
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(UIEventListener))]
    public class Blur : MonoBehaviour
    {

        Image _img;

        UIEventListener _eventListener;

        public UIEventListener EventListener
        {
            get
            {
                return _eventListener;
            }
        }

        private void Awake()
        {
            _img = GetComponent<Image>();
            _eventListener = GetComponent<UIEventListener>();
        }

        private void OnEnable()
        {
            
        }

        /// <summary>
        /// 设置色彩
        /// </summary>
        public Color Color
        {
            get
            {
                return _img.color;
            }

            set
            {
                _img.color = value;
            }
        }

        /// <summary>
        /// 设置遮罩的颜色
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        public void SetColor(float r, float g, float b, float a = 1)
        {
            Color c = new Color(r, g, b, a);
            _img.color = c;
        }

        /// <summary>
        /// 设置遮罩透明度
        /// </summary>
        /// <param name="a"></param>
        public void SetAlpha(float a)
        {
            Color c = _img.color;
            c.a = a;
            _img.color = c;
        }
    }
}