using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    public class GUIText 
    {
        /// <summary>
        /// 创建大标题
        /// </summary>
        /// <param name="title"></param>
        static public void LayoutHead(string title)
        {
            GUILayout.Space(5);
            GUIStyle gs = new GUIStyle();
            //gs.fontStyle = FontStyle.Bold;
            gs.fontSize = 14;
            EditorGUILayout.LabelField(string.Format("<color=#FF8962>--------{0}--------</color>", title), gs);
            GUILayout.Space(5);
        }

        /// <summary>
        /// 用来划分参数类别
        /// </summary>
        /// <param name="title"></param>
        static public void LayoutSplit(string title)
        {
            GUILayout.Space(10);
            GUIStyle gs = new GUIStyle();
            gs.fontSize = 12;
            //gs.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField(string.Format("<color=#488AF3>>>>>>>{0}</color>", title), gs);
        }
    }
}