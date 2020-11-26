using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zero;
using ZeroHot;

namespace ILDemo
{
    class HelpWin : AView
    {
        Sprite[] _tipSpriteList;
        String[] _tipStrList;        
        Image _tip;
        Text _textTip;
        Button _btnClose;

        int _page = 0;

        protected override void OnDisable()
        {
            UIEventListener.Get(gameObject).onClick -= ShowNextPage;
            _btnClose.onClick.RemoveListener(Destroy);
            
        }

        protected override void OnEnable()
        {
            UIEventListener.Get(gameObject).onClick += ShowNextPage;
            _btnClose.onClick.AddListener(Destroy);
        }

        protected override void OnInit(object data)
        {
            _tip = GetComponent<Image>();
            var resData = gameObject.GetComponent<SpriteBindingData>();
            var list = resData.Find("tips");
            _tipSpriteList = list;
            var stringData = gameObject.GetComponent<StringBindingData>();
            _tipStrList = stringData.Find("tips");
            _btnClose = GetChildComponent<Button>("BtnClose");
            _textTip = GetChildComponent<Text>("TextTip");            
        }
        
        void ShowNextPage(PointerEventData e)
        {           
            _page++;
            if(_page >= _tipStrList.Length)
            {
                return;
            }
            else if (_page < 0)
            {
                _page = 0;
            }

            _tip.sprite = _tipSpriteList[_page];
            _textTip.text = _tipStrList[_page];
        }
    }
}
