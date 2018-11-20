using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Zero
{
    /// <summary>
    /// 显示在界面上的Log信息
    /// </summary>
    public class LogMsg : MonoBehaviour
    {
        public static List<string> logRecords = new List<string>();
        const int RECORD_COUNT = 30;

        GameObject _textPrefab;
        Transform _textContent;      


        private void Awake()
        {
            _textPrefab = transform.Find("TextPrefab").gameObject;
            _textContent = transform.Find("Texts");            
        }

        void Start()
        {            
            PointerClickEvent.Get(gameObject).onEvent += OnClick;
        }

        public void SetContent(string msg)
        {
            logRecords.Insert(0, msg);
            CreateText(msg);
            RefreshText();
        }        

        void RefreshText()
        {                        
            if (logRecords.Count > RECORD_COUNT)
            {
                logRecords.RemoveRange(RECORD_COUNT, logRecords.Count - RECORD_COUNT);
            }

            int overCount = _textContent.childCount - RECORD_COUNT;
            while(--overCount > -1)
            {
                GameObject.Destroy(_textContent.GetChild(RECORD_COUNT + overCount).gameObject);
            }            
        }

        private void OnClick(PointerEventData obj)
        {
            GameObject.Destroy(gameObject);
        }

        void CreateText(string content)
        {
            var text = GameObject.Instantiate(_textPrefab, _textContent);
            text.transform.SetAsFirstSibling();
            var wh = text.GetComponent<RectTransform>().sizeDelta;
            wh.x = Screen.width;
            text.GetComponent<RectTransform>().sizeDelta = wh;
            text.GetComponent<Text>().text = content;            
        }
    }
}