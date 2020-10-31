using System.IO;
using UnityEditor;
using Zero;

namespace ZeroEditor
{
    /// <summary>
    /// Zero框架编辑器菜单
    /// </summary>
    public class ToolbarEditorMenu
    {
        [MenuItem("Zero/热更新", false, 0)]
        public static void HotResBuild()
        {            
            BuildHotResEditorWin.Open();
        }

        [MenuItem("Zero/配置文件编辑", false, 50)]
        public static void Configs()
        {
            HotConfigEditorWin.Open("配置文件编辑");
        }

        [MenuItem("Zero/自动生成代码", false, 100)]
        public static void GenerateCode()
        {
            GenerateCodeEditorWin.Open();            
        }

        [MenuItem("Zero/项目自动化配置", false, 150)]
        public static void IosProjectInit()
        {
            PorjectInitEditorWin.Open();
        }

        [MenuItem("Zero/资源优化", false, 200)]
        public static void PackingTagEdit()
        {
            AssetsOptimizesEditorWin.Open();
        }

        [MenuItem("Zero/调试/清理[Caches]目录", false, 250)]
        public static void ClearCachesDir()
        {
            var cacheDir = new DirectoryInfo(ZeroConst.PERSISTENT_DATA_PATH);
            if (cacheDir.Exists)
            {
                cacheDir.Delete(true);
            }            
        }

        [MenuItem("Zero/调试/打开[Caches]目录", false, 300)]
        public static void OpenCachesDir()
        {
            var cacheDir = new DirectoryInfo(ZeroConst.PERSISTENT_DATA_PATH);
            if (cacheDir.Exists)
            {
                ZeroEditorUtil.OpenDirectory(cacheDir.FullName);
            }
        }

        [MenuItem("Zero/调试/GC", false, 350)]
        public static void GC()
        {
            ResMgr.Ins.DoGC();            
        }

        [MenuItem("Zero/工具/位图字体创建", false, 400)]
        public static void CreateBitmapFontGUITools()
        {
            BitmapFontCreaterMenu.CreateBitmapFontGUITools();            
        }

        [MenuItem("Zero/About", false, 450)]
        public static void Document()
        {
            AboutEditorWin.Open();            
        }
    }
}