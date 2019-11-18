using System.IO;
using UnityEditor;
using Zero;

namespace ZeroEditor
{
    /// <summary>
    /// Zero框架编辑器菜单
    /// </summary>
    public class EditorMenu
    {
        [MenuItem("Zero/热更新", false, 0)]
        public static void HotResBuild()
        {            
            BuildHotResEditorWin.Open();
        }

        [MenuItem("Zero/数据配置编辑", false, 100)]
        public static void Configs()
        {
            HotConfigEditorWin.Open("数据配置编辑");
        }

        [MenuItem("Zero/自动生成代码", false, 200)]
        public static void GenerateCode()
        {
            GenerateCodeEditorWin.Open();            
        }

        [MenuItem("Zero/项目自动化配置", false, 300)]
        public static void IosProjectInit()
        {
            PorjectInitEditorWin.Open();
        }

        [MenuItem("Zero/资源优化", false, 401)]
        public static void PackingTagEdit()
        {
            AssetsOptimizesEditorWin.Open();
        }

        [MenuItem("Zero/调试/清理[Caches]目录", false, 601)]
        public static void ClearCachesDir()
        {
            var cacheDir = new DirectoryInfo(ZeroConst.PERSISTENT_DATA_PATH);
            if (cacheDir.Exists)
            {
                cacheDir.Delete(true);
            }            
        }

        [MenuItem("Zero/调试/GC", false, 602)]
        public static void GC()
        {
            ResMgr.Ins.DoGC();            
        }

        [MenuItem("Zero/About", false, 800)]
        public static void Document()
        {
            AboutEditorWin.Open();            
        }
    }
}