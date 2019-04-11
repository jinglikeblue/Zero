using UnityEditor;
using UnityEngine;
using Zero;

[CustomEditor(typeof(Preload))]
public class PreloadCustomEditor : Editor
{
    Preload _target;
    RuntimeVO _vo;

    private void OnEnable()
    {
        _target = target as Preload;
        _vo = _target.runtimeCfg;        
    }

    public override void OnInspectorGUI()
    {
        _vo.isLogEnable = EditorGUILayout.Toggle("是否打印日志", _vo.isLogEnable);
        EditorGUILayout.Space();
        _vo.mainPrefab = EditorGUILayout.TextField("启动Prefab", _vo.mainPrefab);

        OnHotResInspectorGUI();
    }

    /// <summary>
    /// 热更资源
    /// </summary>
    void OnHotResInspectorGUI()
    {
        EditorGUILayout.Space();
        _vo.isHotResProject = EditorGUILayout.Toggle("使用热更", _vo.isHotResProject);
        if (_vo.isHotResProject)
        {
            EditorGUI.indentLevel = 1;

            EditorGUILayout.Space();
            _vo.hotResMode = (EHotResMode)EditorGUILayout.Popup("资源来源", (int)_vo.hotResMode, new string[] { "从网络资源目录获取资源", "从本地资源目录获取资源", "从Resources下直接获取资源（推荐开发阶段使用）" });

            if (EHotResMode.NET == _vo.hotResMode)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("网络资源的根目录");
                _vo.netRoot = EditorGUILayout.TextField(_vo.netRoot);
            }

            if (EHotResMode.LOCAL == _vo.hotResMode)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("本地资源的根目录（建议和发布配置匹配）");
                _vo.localResRoot = EditorGUILayout.TextField(_vo.localResRoot);
            }


            OnDllInspectorGUI();
        }
        else
        {
            EditorGUILayout.Space();
            GUIStyle gs = new GUIStyle();
            //gs.fontStyle |= FontStyle.Bold;
            gs.fontSize = 12;
            EditorGUILayout.LabelField("<color=#FF0000>使用ResMgr时资源将从Resources中获取</color>",gs);
        }
    }

    /// <summary>
    /// 热更代码配置
    /// </summary>
    void OnDllInspectorGUI()
    {
        EditorGUILayout.Space();
        _vo.isUseDll = EditorGUILayout.Toggle("使用DLL", _vo.isUseDll);
        EditorGUI.indentLevel = 2;
        if (_vo.isUseDll)
        {
            EditorGUILayout.Space();
            _vo.ilType = (EILType)EditorGUILayout.Popup("DLL执行方式", (int)_vo.ilType, new string[] { "ILRuntime框架", "反射执行(IL2CPP下会自动切换为ILRuntime)" });

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("文件目录(相对于资源根目录)");
            _vo.fileDir = EditorGUILayout.TextField(_vo.fileDir);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Dll文件名");
            _vo.fileName = EditorGUILayout.TextField(_vo.fileName);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("启动类(完全限定)");
            _vo.className = EditorGUILayout.TextField("Startup Class:", _vo.className);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("启动方法(必须为Static)");
            _vo.methodName = EditorGUILayout.TextField("Startup Method:", _vo.methodName);

            if (_vo.ilType == EILType.IL_RUNTIME)
            {
                EditorGUILayout.Space();
                _vo.isDebugIL = EditorGUILayout.Toggle("调试功能", _vo.isDebugIL);

                EditorGUILayout.Space();
                _vo.isLoadPdb = EditorGUILayout.Toggle("加载Pdb文件", _vo.isLoadPdb);
            }
        }        
    }
}
