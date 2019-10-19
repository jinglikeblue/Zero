using Jing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
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

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                path = path.Replace("/", "\\");
                if (!Directory.Exists(path))
                {
                    Debug.LogError("No Directory: " + path);
                    return;
                }

                System.Diagnostics.Process.Start("explorer.exe", path);
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