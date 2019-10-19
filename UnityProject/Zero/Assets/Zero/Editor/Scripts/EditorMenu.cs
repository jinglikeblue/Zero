using Jing;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    /// <summary>
    /// Zero框架编辑器菜单
    /// </summary>
    public class EditorMenu
    {
        [MenuItem("Zero/发布/setting.json配置", false, 0)]
        public static void Setting()
        {
            SettingEditorWin.Open();
        }

        [MenuItem("Zero/发布/热更资源", false, 100)]
        public static void PublishHotRes()
        {
            HotResBuildEditorWin.Open();
        }

        [MenuItem("Zero/发布/自动生成代码", false, 200)]
        public static void GenerateAssetBundleName()
        {
            GenerateCodeEditorWin.Open();            
        }

        [MenuItem("Zero/iOSProjectInit", false, 300)]
        public static void IosProjectInit()
        {
            IOSProjectInitEditorWin.Open();
        }

        [MenuItem("Zero/调试/清理[Caches]目录", false, 400)]
        public static void ClearCachesDir()
        {
            var root = Directory.GetParent(Application.dataPath).FullName;
            var cacheDirPath = FileSystem.CombineDirs(false, root, "Caches");
            Directory.Delete(cacheDirPath, true);
        }

        [MenuItem("Zero/调试/GC", false, 401)]
        public static void GC()
        {
            ResMgr.Ins.DoGC();
        }

        [MenuItem("Zero/资源/Packing Tag 管理", false, 501)]
        public static void PackingTagEdit()
        {
            PackingTagEditorWin.Open();
        }

        [MenuItem("Zero/资源/查找无用的资源", false, 502)]
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

        [MenuItem("Zero/About", false, 800)]
        public static void Document()
        {
            AboutEditorWin.Open();            
        }
    }
}