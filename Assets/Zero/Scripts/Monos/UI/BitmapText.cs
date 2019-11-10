using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


namespace Zero
{
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public class BitmapText : MonoBehaviour
    {
        [Header("位图纹理UGUI")]
        public Sprite[] sprites;
        [Header("位图字符(顺序和纹理对应)")]
        public string charsTxt;
        [Header("位图样本")]
        public GameObject sample;

        [Header("初始化文本")]
        public string initText = "";

        bool _isDirty = false;
        List<GameObject> _charPool = new List<GameObject>();
        [SerializeField]
        private bool userSelfSize = false;

        string _text;

        public string Text
        {
            get { return _text; }
            set { SetText(value); }
        }

        void Start()
        {
            sample.SetActive(false);
            gameObject.GetComponent<HorizontalLayoutGroup>().childForceExpandWidth = false;
            gameObject.GetComponent<HorizontalLayoutGroup>().childForceExpandHeight = false;

            if (_text == null)
            {
                Text = initText;
            }
        }

        void Update()
        {
            if (_isDirty)
            {
                _isDirty = false;
                Refresh();
            }
        }

        void Refresh()
        {
            Clear();
            var chars = _text.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                int idx = charsTxt.IndexOf(c);
                if (idx < 0 || idx >= sprites.Length)
                {
                    continue;
                }

                var img = GetCharImg();               
                img.name = c.ToString();
                img.SetActive(true);
                var layoutEle = img.GetComponent<LayoutElement>();

                if (img.GetComponent<Image>() != null)
                {
                    Image image = img.GetComponent<Image>();
                    Sprite s = sprites[idx];
                    image.sprite = s;
                    image.SetNativeSize();
                    if (layoutEle != null && s != null && !userSelfSize)
                    {
                        layoutEle.preferredWidth = s.rect.width;
                        layoutEle.preferredHeight = s.rect.height;
                    }
                }

                img.transform.localScale = Vector3.one;
                img.transform.localPosition = Vector3.zero;
            }

            for (int i = 0; i < _charPool.Count; i++)
            {
                Destroy(_charPool[i]);
            }
            _charPool.Clear();
        }

        /// <summary>
        /// 设置文本内容
        /// </summary>
        /// <param name="content">内容</param>
        void SetText(string content)
        {
            if (_text == content)
            {
                return;
            }

            _text = content;
            _isDirty = true;
        }

        GameObject GetCharImg()
        {
            GameObject img = null;
            if (_charPool.Count > 0)
            {
                img = _charPool[0];
                _charPool.RemoveAt(0);
                img.transform.SetAsLastSibling();
            }
            else
            {
                img = Instantiate(sample);
                img.transform.SetParent(gameObject.transform);
            }
            return img;
        }

        void Clear()
        {
            for (var i = 1; i < gameObject.transform.childCount; i++)
            {
                _charPool.Add(gameObject.transform.GetChild(i).gameObject);
            }
        }
    }
}
