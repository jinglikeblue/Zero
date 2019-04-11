using UnityEditor;
using UnityEngine;
using Zero;

[CustomEditor(typeof(Preload))]
public class PreloadCustomEditor : Editor
{
    Preload _target;
    RuntimeVO _vo;

    bool _isUseHotRes = false;
    int _resFrom;
    private void OnEnable()
    {
        _target = target as Preload;
        _vo = _target.runtimeCfg;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        _vo.logEnable = EditorGUILayout.Toggle("是否打印日志", _vo.logEnable);
        EditorGUILayout.Space();
        _vo.mainPrefab.abName = EditorGUILayout.TextField("启动Prefab", _vo.mainPrefab.abName);


        OnHotResInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            Debug.LogFormat("修改了");
        }
    }

    /// <summary>
    /// 热更资源
    /// </summary>
    void OnHotResInspectorGUI()
    {
        EditorGUILayout.Space();
        _isUseHotRes = EditorGUILayout.Toggle("使用热更", _isUseHotRes);
        if (_isUseHotRes)
        {
            EditorGUI.indentLevel = 1;

            EditorGUILayout.Space();
            _resFrom = EditorGUILayout.Popup("资源来源", _resFrom, new string[] { "网络资源", "本地资源", "Resources资源" });

            if (0 == _resFrom)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("网络资源的根目录");
                _vo.netRoot = EditorGUILayout.TextField(_vo.netRoot);
            }

            if (1 == _resFrom)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("本地资源的根目录（建议和发布配置匹配）");
                _vo.developResRoot = EditorGUILayout.TextField(_vo.developResRoot);
            }


            OnDllInspectorGUI();
        }
    }

    /// <summary>
    /// 热更代码配置
    /// </summary>
    void OnDllInspectorGUI()
    {
        EditorGUILayout.Space();
        _vo.ilCfg.isUseDll = EditorGUILayout.Toggle("使用DLL", _vo.ilCfg.isUseDll);
        EditorGUI.indentLevel = 2;
        if (_vo.ilCfg.isUseDll)
        {
            EditorGUILayout.Space();
            _vo.ilCfg.ilType = (RuntimeVO.EILType)EditorGUILayout.Popup("DLL执行方式", (int)_vo.ilCfg.ilType, new string[] { "ILRuntime框架", "反射执行(IL2CPP下会自动切换为ILRuntime)" });

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("文件目录(相对于资源根目录)");
            _vo.ilCfg.fileDir = EditorGUILayout.TextField(_vo.ilCfg.fileDir);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Dll文件名");
            _vo.ilCfg.fileName = EditorGUILayout.TextField(_vo.ilCfg.fileName);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("启动类(完全限定)");
            _vo.ilCfg.className = EditorGUILayout.TextField("Startup Class:", _vo.ilCfg.className);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("启动方法(必须为Static)");
            _vo.ilCfg.methodName = EditorGUILayout.TextField("Startup Method:", _vo.ilCfg.methodName);

            if (_vo.ilCfg.ilType == RuntimeVO.EILType.IL_RUNTIME)
            {
                EditorGUILayout.Space();
                _vo.ilCfg.isDebugIL = EditorGUILayout.Toggle("调试功能", _vo.ilCfg.isDebugIL);

                EditorGUILayout.Space();
                _vo.ilCfg.isLoadPdb = EditorGUILayout.Toggle("加载Pdb文件", _vo.ilCfg.isLoadPdb);
            }
        }
    }
}
