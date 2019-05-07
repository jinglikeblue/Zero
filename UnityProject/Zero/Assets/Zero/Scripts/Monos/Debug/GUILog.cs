using System.Collections.Generic;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 显示在界面上的Log信息
    /// </summary>
    public class GUILog : MonoBehaviour
    {
        public static GUILog _ins;

        public static void Show(string content)
        {
            if (null == _ins)
            {
                const string NAME = "GUILog";
                GameObject go = new GameObject();
                go.name = NAME;
                _ins = go.AddComponent<GUILog>();                
                DontDestroyOnLoad(go);
            }

            _ins.SetContent(content);
        }

        public static void Close()
        {
            if (null != _ins)
            {
                GameObject.Destroy(_ins.gameObject);
                _ins = null;                
            }
        }

        public static void Clear()
        {
            logRecords.Clear();
            if (null != _ins)
            {
                _ins.RefreshText();
            }
        }

        public static List<string> logRecords = new List<string>();
        const int RECORD_COUNT = 30;

        private void OnGUI()
        {
            Event e = Event.current;
            if (e.isMouse)
            {
                Close();
            }
            else
            {
                GUIStyle style = new GUIStyle();
                style.fontSize = 20;
                style.normal.textColor = Color.green;
                GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
                GUILayout.BeginArea(new Rect(200, 0, Screen.width - 200, Screen.height));
                GUILayout.BeginVertical();
                for (int i = 0; i < logRecords.Count; i++)
                {
                    GUILayout.Label(logRecords[i], style);
                }
                GUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }

        void SetContent(string msg)
        {
            logRecords.Insert(0, msg);
            RefreshText();
        }        

        void RefreshText()
        {                        
            if (logRecords.Count > RECORD_COUNT)
            {
                logRecords.RemoveRange(RECORD_COUNT, logRecords.Count - RECORD_COUNT);
            }          
        }
    }
}