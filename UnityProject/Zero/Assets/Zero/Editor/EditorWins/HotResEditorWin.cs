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
            _cfg = _model.CfgVO;
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

            _cfg.abHotResDir = GUIFolderSelect.OnGUI("热更资源目录:",80, _cfg.abHotResDir, ZeroEditorUtil.ResourcesFolder, "hot_res", (path) =>
            {
                path = "Assets" + path.Replace(Application.dataPath, "");
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
                    Copy2DllProj();
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

            if (GUILayout.Button("发布热更资源", GUILayout.Width(200)))
            {
                GUIUtility.ExitGUI();
            }

            EditorGUILayout.EndHorizontal();
        }


        /// <summary>
        /// 拷贝代码到Proj项目
        /// </summary>
        private void Copy2DllProj()
        {
            string projCodeDir = Path.Combine(_cfg.ilProjDir, "codes");

            if (Directory.Exists(_cfg.ilScriptDir))
            {
                if (Directory.Exists(projCodeDir))
                {
                    Directory.Delete(projCodeDir, true);
                }
                FileUtil.CopyFileOrDirectory(_cfg.ilScriptDir, projCodeDir);
                Jing.FileSystem.DeleteFilesByExt(projCodeDir, "meta");

                AssetDatabase.Refresh();
            }
            else
            {
                ShowNotification(new GUIContent("文件夹不存在"));
            }
        }
    }
}
