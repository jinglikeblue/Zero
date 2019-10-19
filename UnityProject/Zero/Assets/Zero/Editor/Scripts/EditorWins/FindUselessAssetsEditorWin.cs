using Jing;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    public class FindUselessAssetsEditorWin : OdinEditorWindow
    {
        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open()
        {
            var win = GetWindow<FindUselessAssetsEditorWin>("查找项目中的无用资源", true);
            win.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 610);
        }

        /// <summary>
        /// 所有有用的资源
        /// </summary>
        HashSet<string> _usefulAssets;

        bool IsUsefulAssetsInited()
        {
            return null != _usefulAssets ? true : false;
        }


        [LabelText("重置扫描库"), Button(size: ButtonSizes.Large), PropertyOrder(-1)]
        void ResetUsefulAssetsSet()
        {
            if (EditorUtility.DisplayDialog("提示", "构建扫描库根据项目大小耗费时间可能很长！", "继续", "取消"))
            {
                _usefulAssets = GetUsefulAssetsSet();
            }
        }

        [LabelText("从选中文件中扫描无用的资源"), Button(size: ButtonSizes.Large), PropertyOrder(-1), ShowIf("IsUsefulAssetsInited")]
        void FindUselessAssets()
        {
            var uselessAssets = GetUselessAssets(_usefulAssets);
            uselessItems.Clear();
            foreach (var asset in uselessAssets)
            {
                var item = new UselessItemVO();
                item.asset = asset;
                uselessItems.Add(item);
            }
        }

        /// <summary>
        /// 没有用的资源列表
        /// </summary>
        [ShowIf("HasUselessAssets"), LabelText("Useless Assets"), ListDrawerSettings(IsReadOnly = true, NumberOfItemsPerPage = 19, Expanded = true)]
        public List<UselessItemVO> uselessItems = new List<UselessItemVO>();

        [HideLabel]
        [Serializable]
        public class UselessItemVO
        {
            [HideInEditorMode]
            public string asset;

            [HorizontalGroup("UselessItem")]
            [ToggleLeft, LabelText("$asset")]
            public bool select = false;

            [HorizontalGroup("UselessItem", width:60)]
            [Button, LabelText("Select"), LabelWidth(60)]
            void Select()
            {
                var ai = AssetImporter.GetAtPath(asset);
                Selection.objects = new UnityEngine.Object[] { ai };
            }
        }

        bool HasUselessAssets()
        {
            if (uselessItems != null && uselessItems.Count > 0)
            {
                return true;
            }
            return false;
        }

        string SelectAllButtonLabel()
        {
            int selectedCount = 0;
            foreach(var item in uselessItems)
            {
                if (item.select)
                {
                    selectedCount++;
                }
            }
            return string.Format("全选 [{0}/{1}]", selectedCount, uselessItems.Count);
        }

        [HorizontalGroup("BottomButtons", 200)]
        [LabelText("$SelectAllButtonLabel"), Button(size: ButtonSizes.Large), ShowIf("HasUselessAssets")]
        void SelectAll()
        {
            //判断是否都是选中状态
            bool isAllSelected = true;

            foreach (var item in uselessItems)
            {
                if(item.select == false)
                {
                    isAllSelected = false;
                    break;
                }
            }                       

            foreach (var item in uselessItems)
            {
                item.select = !isAllSelected;
            }
        }        

        [HorizontalGroup("BottomButtons")]
        [LabelText("删除选中的资源"), Button(size: ButtonSizes.Large), ShowIf("HasUselessAssets")]
        void DeleteSelected()
        {
            List<UselessItemVO> toDelList = new List<UselessItemVO>();
            int i = uselessItems.Count;            
            while (--i > -1)
            {
                var item = uselessItems[i];
                if (item.select)
                {
                    toDelList.Add(item);
                }
            }            

            float selectCount = toDelList.Count;
            var processIdx = 0;
            foreach (var item in toDelList)
            {
                processIdx++;
                FileUtil.DeleteFileOrDirectory(item.asset);
                var title = string.Format("已删除...[{0}/{1}]", processIdx, selectCount);
                EditorUtility.DisplayProgressBar(title, item.asset, processIdx / selectCount);

                uselessItems.Remove(item);
            }
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();            
        }

        List<string> GetUselessAssets(HashSet<string> usefulAssets)
        {
            var uselessAssets = new List<string>();
            var objs = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.DeepAssets);

            for (int i = 0; i < objs.Length; i++)
            {
                var obj = objs[i];
                var type = obj.GetType();
                if (obj is DefaultAsset)
                {
                    //文件夹不处理
                    continue;
                }

                if (obj is MonoScript)
                {
                    //脚本代码不处理
                    continue;
                }

                if (obj is SceneAsset)
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

                file = FileSystem.StandardizeBackslashSeparator(file);
                file = file.Replace(Application.dataPath + "/", "");

                var ai = AssetImporter.GetAtPath(FileSystem.CombinePaths("Assets", file));
                if (null != ai)
                {
                    if (ai.assetBundleName != "")
                    {
                        //是AB资源，有用
                        sets.Add(ai.assetPath);
                    }

                    string[] depends = AssetDatabase.GetDependencies(ai.assetPath);
                    foreach (var depend in depends)
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
