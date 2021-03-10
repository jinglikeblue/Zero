using Jing;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ZeroEditor
{
    public class ZeroEditorUtil : EditorWindow
    {
        /// <summary>
        /// 打开目录
        /// </summary>
        /// <param name="path"></param>
        public static void OpenDirectory(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            var fullPath = FileUtility.CombineDirs(true, ZeroEditorConst.PROJECT_PATH, path);
            if (!Directory.Exists(fullPath))
            {
                Debug.LogError("[无法打开文件夹]不存在: " + fullPath);
                return;
            }

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                fullPath = FileUtility.StandardizeSlashSeparator(fullPath);
                System.Diagnostics.Process.Start("explorer.exe", fullPath);
            }
            else if(Application.platform == RuntimePlatform.OSXEditor)
            {
                var args = string.Format("{0} {1}", "Tools/Mac/OpenDir.sh", fullPath);
                System.Diagnostics.Process.Start("bash", args);
            }
        }

        /// <summary>
        /// 显示编辑器Tip信息.
        /// </summary>
        /// <param name="content"></param>
        [Obsolete("建议使用EditorWindowExtensions.ShowTip扩展方法")]
        public static void ShowTip(EditorWindow editorWin, string content)
        {
            editorWin.ShowTip(content);
        }

        /// <summary>
        /// 获取全部选中物体的路径 
        /// </summary>
        /// <param name="isAbsolutePath">是否获取的是绝对路径</param>
        /// <returns></returns>
        public static string[] GetSelectedObjectPathList(bool isAbsolutePath = false)
        {
            string[] paths = new string[Selection.objects.Length];
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                paths[i] = GetAssetAbsolutePath(AssetDatabase.GetAssetPath(Selection.objects[i]));
            }
            return paths;
        }

        /// <summary>  
        /// 获取资源的绝对路径  
        /// </summary>  
        /// <param name="path">Assets/Editor/...</param>  
        /// <returns></returns>  
        public static string GetAssetAbsolutePath(string assetPath)
        {
            string m_path = Application.dataPath;
            m_path = m_path.Substring(0, m_path.Length - 6);
            m_path += assetPath;
            return m_path;
        }
    }
}