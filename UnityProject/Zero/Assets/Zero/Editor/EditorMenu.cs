using Jing;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    /// <summary>
    /// Zero框架编辑器菜单
    /// </summary>
    public class EditorMenu : AEditorWin
    {
        [MenuItem("Zero/Publish/Setting", false, 0)]
        public static void Setting()
        {
            SettingEditorWin.Open();
        }

        [MenuItem("Zero/Publish/AssetBundle", false, 100)]
        public static void AssetBundle()
        {
            ABEditorWin.Open();
        }

        [MenuItem("Zero/Publish/DLL", false, 101)]
        public static void ILRuntimeDLL()
        {
            DllEditorWin.Open();
        }

        [MenuItem("Zero/Publish/Res", false, 200)]
        public static void Res()
        {
            ResEditorWin.Open();                                 
        }        

        [MenuItem("Zero/iOS/ProjectInit", false, 300)]
        public static void IosProjectInit()
        {
            IOSProjectInitEditorWin.Open();
        }

        [MenuItem("Zero/Debug/Res/Clear [Caches] Dir", false, 400)]
        public static void ClearCachesDir()
        {
            var root = Directory.GetParent(Application.dataPath).FullName;
            var cacheDirPath = FileSystem.CombineDirs(false, root, "Caches");
            Directory.Delete(cacheDirPath, true);
        }

        [MenuItem("Zero/Debug/GC", false, 401)]
        public static void GC()
        {
            ResMgr.Ins.DoGC();
        }

        [MenuItem("Zero/Assets/Sprite/Packing Tag Delete", false, 501)]
        public static void PackingTagEdit()
        {
            PackingTagEditorWin.Open();
        }

        [MenuItem("Zero/Assets/Find Useless", false, 502)]
        public static void FindUselessAssets()
        {
            FindUselessAssetsEditorWin.Open();
        }

        [MenuItem("Zero/IL2CPP/Linker.xml Create", false, 6000)]
        public static void LinkerXMLCreate()
        {
            LinkerXMLEditorWin.Open();
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

        /// <summary>
        /// 编辑器生成的配置文件保存目录
        /// </summary>
        /// <returns></returns>
        public static string GetConfigDir()
        {            
            DirectoryInfo temp = Directory.GetParent(Application.dataPath);
            string dir = Path.Combine(temp.FullName, "EditorConfig");
            if(!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
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

        public static string PlatformDirName
        {
            get {
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


    }
}