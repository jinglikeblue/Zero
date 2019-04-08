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

        [MenuItem("Zero/Publish/HotRes", false, 100)]
        public static void PublishHotRes()
        {
            HotResEditorWin.Open();
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

        [MenuItem("Zero/IL2CPP/Create [Link.xml]", false, 600)]
        public static void LinkerXMLCreate()
        {
            LinkXMLEditorWin.Open();
        }

        [MenuItem("Zero/ILRuntime", false, 700)]
        public static void ILRuntimeEdit()
        {
            ILRuntimeEditorWin.Open();
        }
    }
}