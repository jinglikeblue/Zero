using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    /// <summary>
    /// 字典编辑组件
    /// </summary>
    public class GUIDictionary
    {
        class KV
        {

            public string k;
            public string v;

            public KV()
            {
                this.k = "";
                this.v = "";
            }

            public KV(string k, string v)
            {
                this.k = k;
                this.v = v;
            }
        }

        bool _isNeedRefresh = false;
        Dictionary<string, string> _dic;
        List<KV> _list = new List<KV>();
        string _keyName;
        string _valueName;

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="dic"></param>
        public void SetData(Dictionary<string, string> dic, string keyName, string valueName)
        {
            _dic = dic;
            _keyName = keyName;
            _valueName = valueName;
            _list.Clear();
            foreach(var entry in dic)
            {
                _list.Add(new KV(entry.Key, entry.Value));
            }
        }

        public void Reload()
        {
            SetData(_dic, _keyName, _valueName);
        }

        public Dictionary<string, string> OnGUI(bool isAsyncDic)
        {
            if (isAsyncDic)
            {
                Sync2Dic();
            }

            if (GUILayout.Button("添加", GUILayout.Width(150)))
            {
                _list.Add(new KV());
            }            

            for (int i = 0; i < _list.Count; i++)            
            {
                var kv = _list[i];
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("删除", GUILayout.Width(100)))
                {
                    _list.Remove(kv);
                    break;
                }

                EditorGUILayout.LabelField(_keyName + ":", GUILayout.MaxWidth(60));
                kv.k = EditorGUILayout.TextField("", kv.k, GUILayout.MaxWidth(200));
                EditorGUILayout.LabelField(_valueName + ":", GUILayout.MaxWidth(60));
                kv.v = EditorGUILayout.TextField("", kv.v);
                EditorGUILayout.EndHorizontal();
            }          

            return _dic;
        }

        private void Sync2Dic()
        {
            _dic.Clear();
            foreach(var kv in _list)
            {
                if(kv.k == "" || kv.v == "")
                {
                    continue;
                }

                _dic[kv.k] = kv.v;
            }
        }
    }
}