using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    public class PackingTagEditorWin : AEditorWin
    {
        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open()
        {
            var win = EditorWindow.GetWindow<PackingTagEditorWin>();
            win.titleContent = new GUIContent("Packing Tag Manager");            
            win.minSize = new Vector2(800, 700);
            win.maxSize = new Vector2(1000, 700);
            win.Show();
        }

        Dictionary<string, List<TextureImporter>> _ptData;

        HashSet<string> _selectKey; 

        Vector2 _pos = Vector2.zero;

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(10);

            if(GUILayout.Button("扫描项目中的 Packing Tag 标记", GUILayout.Height(30)))
            {
                _selectKey = new HashSet<string>();
                _ptData = new FindAllPackingTagCommand().Excute();
            }

            if (null != _ptData)
            {
                _pos = GUILayout.BeginScrollView(_pos);
                foreach (var key in _ptData.Keys)
                {
                    if (GUILayout.Toggle(_selectKey.Contains(key), key))
                    {
                        _selectKey.Add(key);
                    }
                    else
                    {
                        _selectKey.Remove(key);
                    }
                }
                GUILayout.EndScrollView();

                if (GUILayout.Button("删除选中的Packing Tag", GUILayout.Height(30)))
                {
                    DeleteSelected();
                }
            }

            GUILayout.EndVertical();
        }

        public void DeleteSelected()
        {
            foreach(var key in _selectKey)
            {
                List<TextureImporter> tiList;
                _ptData.TryGetValue(key, out tiList);
                if (null != tiList)
                {
                    foreach (var ti in tiList)
                    {
                        ti.spritePackingTag = string.Empty;
                    }
                }
            }

            _ptData = new FindAllPackingTagCommand().Excute();
        }        
    }
}