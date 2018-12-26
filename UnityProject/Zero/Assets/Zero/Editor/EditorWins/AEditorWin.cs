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
        /// AB资源的保存目录
        /// </summary>
        public const string AB_DIR = "ab";

        const string CONFIG_NAME = "editor_config.json";

        /// <summary>
        /// 编辑器生成的配置文件保存目录
        /// </summary>
        /// <returns></returns>
        static string ConfigDir
        {
            get
            {
                DirectoryInfo temp = Directory.GetParent(Application.dataPath);
                string dir = FileSystem.CombineDirs(true, temp.FullName, "EditorConfig", EditorMenu.PlatformDirName);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                return dir;
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="data">配置的数据</param>
        /// <param name="fileName">文件名</param>
       public static void SaveConfig(object data, string fileName)
        {
            string json = JsonMapper.ToJson(data);
            File.WriteAllText(Path.Combine(ConfigDir, fileName), json);
        }

        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">配置文件名称</param>
        /// <returns></returns>
        public static T LoadConfig<T>(string fileName)
        {
            string path = Path.Combine(ConfigDir, fileName);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return JsonMapper.ToObject<T>(json);
            }
            return default(T);
        }

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

        /// <summary>
        /// 生成字典的编辑界面
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="keyName"></param>
        /// <param name="valueName"></param>
        protected void GUILayoutDictionary(Dictionary<string,string> dic, string keyName, string valueName)
        {
            if (GUILayout.Button("添加", GUILayout.Width(150)))
            {
                dic[""] = "";
            }

            foreach (var entry in dic)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("删除", GUILayout.Width(100)))
                {
                    dic.Remove(entry.Key);
                    break;
                }

                EditorGUILayout.LabelField(keyName + ":", GUILayout.MaxWidth(60));
                string newKey = EditorGUILayout.TextField("", entry.Key, GUILayout.MaxWidth(200));
                EditorGUILayout.LabelField(valueName + ":", GUILayout.MaxWidth(60));
                string newValue = EditorGUILayout.TextField("", entry.Value);
                EditorGUILayout.EndHorizontal();

                if (newKey != entry.Key)
                {
                    dic.Remove(entry.Key);
                    dic[newKey] = newValue;
                    break;
                }
                else if (newValue != entry.Value)
                {
                    dic[newKey] = newValue;
                    break;
                }
            }
        }
    }
}