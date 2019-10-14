using Jing;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    public class ZeroEditorUtil
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
    }
}