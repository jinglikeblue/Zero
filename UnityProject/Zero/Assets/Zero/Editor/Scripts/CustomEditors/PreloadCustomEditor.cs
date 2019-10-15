using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Zero.Edit
{
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
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space();
            _vo.isLogEnable = EditorGUILayout.Toggle("是否打印日志", _vo.isLogEnable);

            EditorGUILayout.Space();
            _vo.mainPrefab = EditorGUILayout.TextField("启动Prefab", _vo.mainPrefab);

            
            EditorGUILayout.Space();                      
            EditorGUILayout.LabelField("启动类");
            EditorGUI.indentLevel = 1;
            _vo.className = EditorGUILayout.TextField("(完全限定) Class:", _vo.className);            
            _vo.methodName = EditorGUILayout.TextField("(静态) Method:", _vo.methodName);

            EditorGUI.indentLevel = 0;
            OnHotResInspectorGUI();

            //当Inspector 面板发生变化时保存数据
            if (EditorGUI.EndChangeCheck())
            {
                EditorSceneManager.MarkSceneDirty(_target.gameObject.scene);
            }
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
                _vo.hotResMode = (EHotResMode)EditorGUILayout.Popup("资源来源", (int)_vo.hotResMode, new string[] { "从网络资源目录加载资源", "从本地资源目录加载资源", "使用AssetDataBase加载资源（推荐开发阶段使用）" });

                if (EHotResMode.NET_ASSET_BUNDLE == _vo.hotResMode)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("网络资源的根目录");
                    _vo.netRoot = EditorGUILayout.TextField(_vo.netRoot);
                }
                else if (EHotResMode.LOCAL_ASSET_BUNDLE == _vo.hotResMode)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("本地资源的根目录(通过菜单Zero > Publish > HotRes中的Res发布目录配置)");
                    //var model = new HotResPublishModel();
                    //_vo.localResRoot = model.Cfg.resDir;
                    if (string.IsNullOrEmpty(_vo.localResRoot))
                    {
                        EditorGUILayout.LabelField("<color=#FF0000>*尚未配置</color>", new GUIStyle());
                    }
                    else
                    {
                        GUI.enabled = false;
                        EditorGUILayout.TextField(_vo.localResRoot);
                        GUI.enabled = true;
                    }
                }
                else if (EHotResMode.ASSET_DATA_BASE == _vo.hotResMode)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Asset中热更资源目录(通过菜单Zero > Publish > HotRes中的AssetBundle配置)");
                    //var model = new HotResPublishModel();                    
                    //_vo.hotResRoot = model.Cfg.abHotResDir;
                    if (string.IsNullOrEmpty(_vo.hotResRoot))
                    {                        
                        EditorGUILayout.LabelField("<color=#FF0000>*尚未配置</color>", new GUIStyle());
                    }
                    else
                    {
                        GUI.enabled = false;
                        EditorGUILayout.TextField(_vo.hotResRoot);
                        GUI.enabled = true;
                    }
                }

                OnDllInspectorGUI();
            }
            else
            {
                EditorGUILayout.Space();
                GUIStyle gs = new GUIStyle();
                //gs.fontStyle |= FontStyle.Bold;
                gs.fontSize = 12;
                EditorGUILayout.LabelField("<color=#FF0000>使用ResMgr时资源将从Resources中加载资源</color>", gs);
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
                GUI.enabled = false;
                EditorGUILayout.TextField(ZeroConst.DLL_DIR_NAME);
                GUI.enabled = true;                

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Dll文件名");
                _vo.fileName = EditorGUILayout.TextField(_vo.fileName);

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
}