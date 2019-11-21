using UnityEditor;
using UnityEngine;
using Zero;
using ZeroEditor;

/// <summary>
/// 自定义右键菜单
/// </summary>
public class CustomRightClickMenu
{
    [MenuItem("Assets/Zero快捷操作/生成DLL", false, 0)]
    static void GenerateDll()
    {
        var cmd = new DllBuildCommand(ZeroEditorConst.HOT_SCRIPT_ROOT_DIR, ZeroEditorConst.DLL_PUBLISH_DIR);
        cmd.onFinished += (DllBuildCommand self, bool isSuccess) => {
            var tip = isSuccess ? "Dll生成成功!" : "Dll生成失败!";
            Debug.Log(Log.Zero1(tip));
        };
        cmd.Execute();        
    }

    [MenuItem("Assets/Zero快捷操作/资源名生成", false, 100)]
    static void GenerateAssetNames()
    {        
        var findCmd = new FindAssetBundlesCommand(false);
        findCmd.onFinished += (cmd, list) =>
        {
            new GenerateABClassCommand(list).Excute();
            new GenerateAutoViewRegisterClassCommand(list, cmd.cfg.viewClassNS).Excute();
            Debug.Log(Log.Zero1("生成完毕!"));
        };
        findCmd.Excute();
    }
}
