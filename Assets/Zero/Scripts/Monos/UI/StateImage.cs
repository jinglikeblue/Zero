using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zero
{
    [RequireComponent(typeof(Image))]
    public class StateImage : MonoBehaviour
    {
        public Sprite[] stateSpriteList;

        public int State { get; private set; } = -1;

        /// <summary>
        /// 管理的图像
        /// </summary>
        public Image Image { get; private set; }

        /// <summary>
        /// 是否切换状态后，自动执行Set Naative Size
        /// </summary>
        public bool isAutoSetNativeSize = false;

        private void Awake()
        {
            Image = GetComponent<Image>();
            if (State != -1)
            {
                SetState(State);
            }
        }

        public void SetState(int i)
        {
            if (null == Image)
            {
                State = i;
                return;
            }

            if (i >= 0 && i < stateSpriteList.Length && stateSpriteList[i] != null)
            {
                Image.sprite = stateSpriteList[i];
                if (Image.sprite != null && isAutoSetNativeSize)
                {
                    Image.SetNativeSize();
                }
            }
        }
    }
}