using Jing;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    public class FindUselessAssetsEditorWin:AEditorWin
    {
        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open()
        {
            var win = EditorWindow.GetWindow<FindUselessAssetsEditorWin>("Useless Assets Find", true);            
            win.minSize = new Vector2(800, 700);
            win.maxSize = new Vector2(1000, 700);
            win.Show();
        }

        Vector2 _pos = Vector2.zero;
        HashSet<string> _usefulAssets;
        List<string> _uselessAssets;        
        HashSet<string> _selectFiles;
        bool _selectAll = false;

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(10);

            if (GUILayout.Button(_usefulAssets == null ? "初始化扫描库" : "重置扫描库", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("提示", "构建扫描库根据项目大小耗费时间可能很长！", "继续", "取消"))
                {
                    _usefulAssets = GetUsefulAssetsSet();
                }
            }

            if (null == _usefulAssets)
            {
                GUILayout.EndVertical();
                return;
            }

            if (GUILayout.Button("从选中文件中扫描无用的资源", GUILayout.Height(30)))
            {
                _selectFiles = new HashSet<string>();
                _pos = Vector2.zero;
                _selectAll = false;
                _uselessAssets = GetUselessAssets(_usefulAssets);
            }
            

            if(null != _uselessAssets && _uselessAssets.Count > 0)
            {
                _pos = GUILayout.BeginScrollView(_pos);
                foreach (var asset in _uselessAssets)
                {
                    if (GUILayout.Toggle(_selectFiles.Contains(asset), string.Format("{0}", asset)))
                    {
                        _selectFiles.Add(asset);
                    }
                    else
                    {
                        _selectFiles.Remove(asset);
                    }
                }
                GUILayout.EndScrollView();


                GUILayout.BeginHorizontal();                
                string label = _selectFiles.Count == _uselessAssets.Count ? "取消全选" : "全选";
                label += string.Format("[{0}/{1}]", _selectFiles.Count, _uselessAssets.Count);
                if (GUILayout.Button(label, GUILayout.Height(30), GUILayout.Width(200)))
                {
                    if(_selectFiles.Count == _uselessAssets.Count)
                    {
                        _selectFiles = new HashSet<string>();
                    }
                    else
                    {
                        foreach(var file in _uselessAssets)
                        {
                            _selectFiles.Add(file);
                        }
                    }
                }

                if (GUILayout.Button("删除选中的资源", GUILayout.Height(30)))
                {
                    DeleteAssets(_selectFiles);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void DeleteAssets(HashSet<string> selectKey)
        {
            float selectCount = selectKey.Count;
            var processIdx = 0;
            foreach (var asset in selectKey)
            {
                processIdx++;
                FileUtil.DeleteFileOrDirectory(asset);
                var title = string.Format("已删除...[{0}/{1}]", processIdx, selectCount);
                EditorUtility.DisplayProgressBar(title, asset, processIdx / selectCount);

                _uselessAssets.Remove(asset);
            }
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
            _selectFiles.Clear();
            
        }

        private List<string> GetUselessAssets(HashSet<string> usefulAssets)
        {
            var uselessAssets = new List<string>();
            var objs = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.DeepAssets);
            
            for (int i = 0; i < objs.Length; i++)
            {
                var obj = objs[i];
                var type = obj.GetType();
                if(obj is DefaultAsset)
                {
                    //文件夹不处理
                    continue;
                }

                if(obj is MonoScript)
                {
                    //脚本代码不处理
                    continue;
                }

                if(obj is SceneAsset)
                {
                    //场景文件不处理
                    continue;
                }

                var file = AssetDatabase.GetAssetPath(obj);                

                var title = string.Format("已扫描...[{0}/{1}]", i + 1, objs.Length);
                EditorUtility.DisplayProgressBar(title, file, i / objs.Length);

                if (false == usefulAssets.Contains(file))
                {
                    uselessAssets.Add(file);
                }
            }

            EditorUtility.ClearProgressBar();

            return uselessAssets;
        }

        HashSet<string> GetUsefulAssetsSet()
        {
            HashSet<string> sets = new HashSet<string>();
            var files = Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories);
            float total = files.Length;
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var title = string.Format("构建中...({0}/{1})", i + 1, files.Length);
                EditorUtility.DisplayProgressBar(title, file, i / total);

                file = FileSystem.StandardizeSeparator(file);
                file = file.Replace(Application.dataPath + "/", "");

                var ai = AssetImporter.GetAtPath(FileSystem.CombinePaths("Assets", file));
                if (null != ai)
                {
                    if(ai.assetBundleName != "")
                    {
                        //是AB资源，有用
                        sets.Add(ai.assetPath);
                    }

                    string[] depends = AssetDatabase.GetDependencies(ai.assetPath);
                    foreach(var depend in depends)
                    {
                        if (depend != ai.assetPath)
                        {
                            //这些被依赖的资源都是有用的
                            sets.Add(depend);
                        }
                    }
                }
            }
            EditorUtility.ClearProgressBar();
            return sets;
        }
    }
}
