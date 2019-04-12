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
        /// 平台目录
        /// </summary>
        public static string PlatformDirName
        {
            get
            {
                string name;
#if UNITY_STANDALONE
                name = "pc/";
#elif UNITY_IPHONE
        name = "ios/";
#elif UNITY_ANDROID
                name = "android/";
#endif
                return name;
            }
        }

        /// <summary>
        /// 项目的Resources目录
        /// </summary>
        public static string ResourcesFolder {
            get
            {
                string resourcesDir = FileSystem.CombineDirs(true, Application.dataPath, "Resources");
                return resourcesDir;
            }
        }

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
        /// 当前发布平台
        /// </summary>
        public static BuildTarget CurrentPlatform
        {
            get
            {
                BuildTarget platform;
#if UNITY_STANDALONE
                platform = BuildTarget.StandaloneWindows;
#elif UNITY_IPHONE
        platform = BuildTarget.iOS;
#elif UNITY_ANDROID
        platform = BuildTarget.Android;
#endif
                return platform;
            }
        }
    }
}