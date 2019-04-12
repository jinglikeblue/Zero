using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    public class GUIFileSelect
    {
        public static string OnGUI(string title, int titleWidth, string file, string dir, string ext, Func<string, string> onSelect = null)
        {
            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(title, GUILayout.Width(titleWidth));
            file = EditorGUILayout.TextField(file);
            if (GUILayout.Button("选择", GUILayout.Width(60)))
            {
                var selectedFile = EditorUtility.OpenFilePanel("选择文件", dir, ext);
                if (false == string.IsNullOrEmpty(selectedFile))
                {
                    if (null != onSelect)
                    {
                        file = onSelect(selectedFile);
                    }
                    else
                    {
                        file = selectedFile;
                    }
                }
            }

            GUILayout.EndHorizontal();
            return file;
        }
    }
}