using Jing;
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

            var fullPath = FileSystem.CombineDirs(true, ZeroEditorConst.PROJECT_PATH, path);
            if (!Directory.Exists(fullPath))
            {
                Debug.LogError("[无法打开文件夹]不存在: " + fullPath);
                return;
            }

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                fullPath = FileSystem.StandardizeSlashSeparator(fullPath);
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
    }
}