using System.IO;
using Jing.IOS;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IPHONE
using UnityEditor.iOS.Xcode;
#endif
using UnityEngine;

/// <summary>
/// XCODE项目的初始化
/// </summary>
public class IOSProjectInit : Editor
{
#if UNITY_IPHONE
    /// <summary>
    /// XCODE项目发布后的处理
    /// </summary>
    /// <param name="target"></param>
    /// <param name="path"></param>
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if(BuildTarget.iOS != target)
        {
            return;
        }

        IOSProjectInitData data = IOSProjectInitData.Load();

        string projPath = PBXProject.GetPBXProjectPath(path);
        PBXProject pbx = new PBXProject();
        pbx.ReadFromString(File.ReadAllText(projPath));
        string guid = pbx.TargetGuidByName("Unity-iPhone");

        //支付宝需要的内容
        pbx.AddFrameworkToProject(guid, "CoreTelephony.framework", false);
        pbx.AddFileToBuild(guid, pbx.AddFile("usr/lib/libz.tbd", "Frameworks/libz.tbd", PBXSourceTree.Sdk));
        pbx.AddFileToBuild(guid, pbx.AddFile("usr/lib/libc++.tbd", "Frameworks/libc++.tbd", PBXSourceTree.Sdk));        

        //微信SDK需要的内容
        pbx.SetBuildProperty(guid, "ENABLE_BITCODE", "NO");
        pbx.SetBuildProperty(guid, "OTHER_LDFLAGS", "-ObjC");
        File.WriteAllText(projPath, pbx.WriteToString());

        ////修改PList
        string plistPath = path + "/Info.plist";
        InfoPListEditor pListEditor = new InfoPListEditor(plistPath);
        pListEditor.Add("FacebookAppID", "1642587459185521");
        pListEditor.Add("FacebookDisplayName", "");
        foreach (string urlScheme in data.urlSchemes)
        {
            pListEditor.AddUrlScheme("yfy", urlScheme);
        }
        foreach (string whiteUrlScheme in data.whiteSchemeList)
        {
            pListEditor.AddLSApplicationQueriesScheme(whiteUrlScheme);
        }        
        pListEditor.Save();        
    }

#endif
}
