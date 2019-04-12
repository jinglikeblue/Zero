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

            IOSProjectInitConfig cfg = new IOSProjectInitConfig();

            string projPath = PBXProject.GetPBXProjectPath(path);
            PBXProject pbx = new PBXProject();
            pbx.ReadFromString(File.ReadAllText(projPath));
            string guid = pbx.TargetGuidByName("Unity-iPhone");

            foreach(var framework in cfg.Cfg.frameworkToProjectList)
            {
                pbx.AddFrameworkToProject(guid, framework, false);
            }
            
            foreach(var entry in cfg.Cfg.file2BuildList)
            {
                pbx.AddFileToBuild(guid, pbx.AddFile(entry.Key, entry.Value, PBXSourceTree.Sdk));
            }

            foreach (var entry in cfg.Cfg.buildPropertyList)
            {
                pbx.SetBuildProperty(guid, entry.Key, entry.Value);
            }

            File.WriteAllText(projPath, pbx.WriteToString());

            ////修改PList
            string plistPath = path + "/Info.plist";
            InfoPListEditor pListEditor = new InfoPListEditor(plistPath);

            foreach (var entry in cfg.Cfg.pListDataList)
            {
                pListEditor.Add(entry.Key, entry.Value);
            }

            foreach (string urlScheme in cfg.Cfg.urlSchemeList)
            {
                pListEditor.AddUrlScheme("ZeroUrlSchemes", urlScheme);
            }

            foreach (string whiteUrlScheme in cfg.Cfg.appQueriesSchemeList)
            {
                pListEditor.AddLSApplicationQueriesScheme(whiteUrlScheme);
            }
            pListEditor.Save();

            //string projPath = PBXProject.GetPBXProjectPath(path);
            //PBXProject pbx = new PBXProject();
            //pbx.ReadFromString(File.ReadAllText(projPath));
            //string guid = pbx.TargetGuidByName("Unity-iPhone");

            ////支付宝需要的内容
            //pbx.AddFrameworkToProject(guid, "CoreTelephony.framework", false);
            //pbx.AddFileToBuild(guid, pbx.AddFile("usr/lib/libz.tbd", "Frameworks/libz.tbd", PBXSourceTree.Sdk));
            //pbx.AddFileToBuild(guid, pbx.AddFile("usr/lib/libc++.tbd", "Frameworks/libc++.tbd", PBXSourceTree.Sdk));

            ////微信SDK需要的内容
            //pbx.SetBuildProperty(guid, "ENABLE_BITCODE", "NO");
            //pbx.SetBuildProperty(guid, "OTHER_LDFLAGS", "-ObjC");
            //File.WriteAllText(projPath, pbx.WriteToString());

            //////修改PList
            //string plistPath = path + "/Info.plist";
            //InfoPListEditor pListEditor = new InfoPListEditor(plistPath);
            //pListEditor.Add("FacebookAppID", "1642587459185521");
            //pListEditor.Add("FacebookDisplayName", "");
            //foreach (string urlScheme in data.urlSchemes)
            //{
            //    pListEditor.AddUrlScheme("ZeroUrlSchemes", urlScheme);
            //}
            //foreach (string whiteUrlScheme in data.whiteSchemeList)
            //{
            //    pListEditor.AddLSApplicationQueriesScheme(whiteUrlScheme);
            //}
            //pListEditor.Save();
        }
    }
}
#endif