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
            var win = EditorWindow.GetWindow<PackingTagEditorWin>("Packing Tag Manager", true);
            //win.titleContent = new GUIContent(, "Packing Tag Manager");
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

            if (GUILayout.Button("扫描项目中的 Packing Tag 标记", GUILayout.Height(30)))
            {
                RefreshPackingTags();
            }

            if (null != _ptData)
            {
                _pos = GUILayout.BeginScrollView(_pos);
                foreach (var key in _ptData.Keys)
                {
                    if (GUILayout.Toggle(_selectKey.Contains(key), string.Format("{0} [{1}]", key, _ptData[key].Count)))
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

                    //if (EditorUtility.DisplayDialog("成功", "是否重新扫描？", "Yes", "No"))
                    //{
                    //    RefreshPackingTags();
                    //}                    
                }
            }

            GUILayout.EndVertical();
        }

        void RefreshPackingTags()
        {
            _selectKey = new HashSet<string>();
            _ptData = new FindAllPackingTagCommand().Excute();
        }

        public void DeleteSelected()
        {
            var selectCount = _selectKey.Count;
            var processIdx = 0;
            foreach (var key in _selectKey)
            {
                processIdx++;
                List<TextureImporter> tiList;
                _ptData.TryGetValue(key, out tiList);
                if (null != tiList)
                {

                    float total = tiList.Count - 1;
                    for (int i = 0; i < tiList.Count; i++)
                    {
                        var ti = tiList[i];
                        var title = string.Format("正在删除Packing Tag[{0}/{1}]: {2}({3}/{4})", processIdx, selectCount, key, i + 1, tiList.Count);
                        EditorUtility.DisplayProgressBar(title, ti.assetPath, i / total);
                        ti.spritePackingTag = string.Empty;
                        ti.SaveAndReimport();
                    }
                    _ptData.Remove(key);
                }
            }
            EditorUtility.ClearProgressBar();
            _selectKey.Clear();
        }
    }
}