using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    public class GUIFolderSelect
    {       
        public static string OnGUI(string title,int titleWidth, string folder, string dir, string defaultName, Func<string, string> onSelect = null)
        {
            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(title, GUILayout.Width(titleWidth));
            folder = EditorGUILayout.TextField(folder);
            if (GUILayout.Button("选择", GUILayout.Width(60)))
            {
                var selectedFolder = EditorUtility.OpenFolderPanel("选择文件", dir, defaultName);
                if (false == string.IsNullOrEmpty(selectedFolder))
                {
                    if (null != onSelect)
                    {
                        folder = onSelect(selectedFolder);
                    }
                    else
                    {
                        folder = selectedFolder;
                    }
                }
            }

            GUILayout.EndHorizontal();
            return folder;
        }
    }
}