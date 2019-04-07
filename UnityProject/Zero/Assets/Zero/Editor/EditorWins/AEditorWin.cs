using Jing;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    public abstract class AEditorWin : EditorWindow
    {
        /// <summary>
        /// 创建大标题
        /// </summary>
        /// <param name="title"></param>
        protected void GUILayoutHead(string title)
        {
            GUILayout.Space(5);
            GUIStyle gs = new GUIStyle();
            //gs.fontStyle = FontStyle.Bold;
            gs.fontSize = 14;
            EditorGUILayout.LabelField(string.Format("<color=#FF0000>--------{0}--------</color>", title), gs);
            GUILayout.Space(5);
        }


        /// <summary>
        /// 用来划分参数类别
        /// </summary>
        /// <param name="title"></param>
        protected void GUILayoutSplit(string title)
        {
            GUILayout.Space(10);
            GUIStyle gs = new GUIStyle();
            gs.fontSize = 12;            
            //gs.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField(string.Format("<color=#0000FF>>>>>>>{0}</color>" ,title),gs);
        }

        /// <summary>
        /// 生成数组编辑界面，编辑完成会返回新的数组
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        protected string[] GUILayoutArray(string[] array)
        {
            int groupSize = EditorGUILayout.IntField("数量:", array.Length, GUILayout.MaxWidth(200));
            if (groupSize != array.Length)
            {
                string[] newArr = new string[groupSize];
                Array.Copy(array, 0, newArr, 0, groupSize < array.Length ? groupSize : array.Length);
                array = newArr;
            }

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = EditorGUILayout.TextField("资源组名称:", array[i]);
            }
            return array;
        }
    }
}