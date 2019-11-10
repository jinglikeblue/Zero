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
        Image _img;

        private void Awake()
        {
            _img = GetComponent<Image>();
        }

        public void SetState(int i)
        {
            if (i >= 0 && i < stateSpriteList.Length && null != _img && stateSpriteList[i] != null)
            {
                _img.sprite = stateSpriteList[i];
            }
        }
    }
}