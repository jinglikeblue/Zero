using System;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    public class GUIArray
    {
        /// <summary>
        /// 生成数组编辑界面，编辑完成会返回新的数组
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        static public string[] LayoutArray(string[] array)
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