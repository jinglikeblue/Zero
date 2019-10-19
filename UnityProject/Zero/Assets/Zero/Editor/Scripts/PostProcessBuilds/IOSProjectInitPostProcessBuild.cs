#if UNITY_IPHONE
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Zero.Edit.IOS
{
    /// <summary>
    /// XCODE项目的初始化
    /// </summary>
    public class IOSProjectInitPostProcessBuild : Editor
    {
        /// <summary>
        /// XCODE项目发布后的处理
        /// </summary>
        /// <param name="target"></param>
        /// <param name="path"></param>
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {
            if (BuildTarget.iOS != target)
            {
                return;
            }

            var cfg = EditorConfigUtil.LoadConfig<IOSProjectInitConfigVO>(IOSProjectInitConfigVO.CONFIG_NAME);

            string projPath = PBXProject.GetPBXProjectPath(path);
            PBXProject pbx = new PBXProject();
            pbx.ReadFromString(File.ReadAllText(projPath));
            string guid = pbx.TargetGuidByName("Unity-iPhone");

            foreach(var framework in cfg.frameworkToProjectList)
            {
                pbx.AddFrameworkToProject(guid, framework, false);
            }
            
            foreach(var entry in cfg.file2BuildList)
            {
                pbx.AddFileToBuild(guid, pbx.AddFile(entry.Key, entry.Value, PBXSourceTree.Sdk));
            }

            foreach (var entry in cfg.buildPropertyList)
            {
                pbx.SetBuildProperty(guid, entry.Key, entry.Value);
            }

            File.WriteAllText(projPath, pbx.WriteToString());

            ////修改PList
            string plistPath = path + "/Info.plist";
            InfoPListEditor pListEditor = new InfoPListEditor(plistPath);

            foreach (var entry in cfg.pListDataList)
            {
                pListEditor.Add(entry.Key, entry.Value);
            }

            foreach (string urlScheme in cfg.urlSchemeList)
            {
                pListEditor.AddUrlScheme("ZeroUrlSchemes", urlScheme);
            }

            foreach (string whiteUrlScheme in cfg.appQueriesSchemeList)
            {
                pListEditor.AddLSApplicationQueriesScheme(whiteUrlScheme);
            }
            pListEditor.Save();
        }
    }
}
#endif