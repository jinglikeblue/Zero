using Jing;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    public class HotResEditorWin : AEditorWin
    {
        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open()
        {
            var win = EditorWindow.GetWindow<HotResEditorWin>("HotRes Manager", true);
            win.minSize = new Vector2(800, 400);
            win.maxSize = new Vector2(1000, 400);
            win.Show();
        }

        HotResPublishModel _model;
        HotResConfigVO _cfg;

        bool _isBuildDLL = true;
        bool _isBuildAB = true;
        bool _isBuildResJson = true;


        private void OnEnable()
        {
            _model = new HotResPublishModel();
            _cfg = _model.Cfg;
        }

        private void OnGUI()
        {            
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("保存配置"))
            {
                _model.SaveConfig();
                ShowNotification(new GUIContent("保存成功"));
            }

            _cfg.resDir = EditorGUILayout.TextField("Res目录:", _cfg.resDir);
            GUILayout.Space(10);

            GUILayoutSplit("AssetBundle配置");
            AssetBundleGUI();

            GUILayoutSplit("DLL配置");
            DllGUI();

            GUILayoutSplit("res.json配置");
            ResJsonGUI();

            GUILayoutSplit("发布");
            BuildGUI();

            EditorGUILayout.EndVertical();
        }

        void AssetBundleGUI()
        {
            _cfg.isKeepManifest = GUILayout.Toggle(_cfg.isKeepManifest, "保留「.manifest」文件", GUILayout.Width(150));

            _cfg.abHotResDir = GUIFolderSelect.OnGUI("热更资源目录(该目录为Resources下的子目录，仅该目录下的资源会作为AssetBundle发布):",500, _cfg.abHotResDir, ZeroEditorUtil.ResourcesFolder, "hot_res", (path) =>
            {
                path = path.Replace(Application.dataPath, "");

                if (false == path.StartsWith("/Resources"))
                {
                    ShowNotification(new GUIContent("请选择Assets/Resources下的目录"));
                    path = "";
                }
                else
                {
                    path = "Assets" + path;
                }                
                return path;
            });
        }

        void DllGUI()
        {
            _cfg.ilScriptDir = GUIFolderSelect.OnGUI("Dll开发目录:", 200, _cfg.ilScriptDir, Application.dataPath, "", (path) =>
            {
                path = "Assets" + path.Replace(Application.dataPath, "");
                return path;
            });

            _cfg.ilProjDir = GUIFolderSelect.OnGUI("Dll项目目录", 200, _cfg.ilProjDir, Application.dataPath, "");

            _cfg.ilProjCsprojPath = GUIFileSelect.OnGUI("Dll项目csproj文件:", 200, _cfg.ilProjCsprojPath, Application.dataPath, "csproj");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Devenv工具地址:", GUILayout.Width(200));
            _cfg.devenvPath = EditorGUILayout.TextField(_cfg.devenvPath);
            GUILayout.EndHorizontal();



            GUILayout.BeginHorizontal();

            if (GUILayout.Button("代码拷贝到项目目录"))
            {
                if (EditorUtility.DisplayDialog("警告！", "是否确认执行(目标目录将被覆盖)", "Yes", "No"))
                {
                    _model.Copy2DllProj();
                    ShowNotification(new GUIContent("完成"));
                }
            }

            if (GUILayout.Button("打开DLL项目目录"))
            {
                ZeroEditorUtil.OpenDirectory(Path.GetDirectoryName(_cfg.ilProjCsprojPath));
            }

            GUILayout.EndHorizontal();
        }

        void ResJsonGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Manifest文件资源路径:", GUILayout.Width(200));
            _cfg.manifestName = EditorGUILayout.TextField(_cfg.manifestName);
            GUILayout.EndHorizontal();
        }

        void BuildGUI()
        {
            EditorGUILayout.BeginHorizontal();

            _isBuildAB = GUILayout.Toggle(_isBuildAB, "AssetBundle", GUILayout.Width(100));
            _isBuildDLL = GUILayout.Toggle(_isBuildDLL, "Dll", GUILayout.Width(50));            
            _isBuildResJson = GUILayout.Toggle(_isBuildResJson, "res.json", GUILayout.Width(100));

            if (GUILayout.Button("发布热更资源"))
            {
                Build();
                GUIUtility.ExitGUI();
            }

            EditorGUILayout.EndHorizontal();
        }

        void Build()
        {
            

            if (_isBuildDLL)
            {
                EditorUtility.DisplayProgressBar("打包热更资源", "开始发布DLL", 0f);
                Debug.Log("开始发布DLL");
                _model.BuildDll();
            }

            if (_isBuildAB)
            {
                EditorUtility.DisplayProgressBar("打包热更资源", "开始发布AssetBundle", 0f);
                Debug.Log("开始发布AssetBundle");                
                //发布AB资源
                _model.BuildAssetBundle();
            }

            if (_isBuildResJson)
            {
                EditorUtility.DisplayProgressBar("打包热更资源", "开始发布Res.json", 0f);
                Debug.Log("开始发布Res.json");
                _model.BuildResJsonFile();
            }

            //打开目录
            ZeroEditorUtil.OpenDirectory(FileSystem.CombineDirs(false, _cfg.resDir, ZeroEditorUtil.PlatformDirName));
            EditorUtility.ClearProgressBar();
        }
    }
}
