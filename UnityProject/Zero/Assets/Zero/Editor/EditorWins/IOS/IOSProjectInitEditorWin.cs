using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zero.Edit;

namespace Zero.Edit
{
    public class IOSProjectInitEditorWin : AEditorWin
    {
        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open()
        {
            var win = EditorWindow.GetWindow<IOSProjectInitEditorWin>();
            win.titleContent = new GUIContent("ProjectInitEditorWin");
            win.minSize = new Vector2(800, 500);
            win.maxSize = new Vector2(1000, 500);
            win.Show();
        }

        IOSProjectInitConfig _tool;
        private void OnEnable()
        {
            _tool = new IOSProjectInitConfig();
        }


        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("保存配置"))
            {
                _tool.SaveCfg();
                ShowNotification(new GUIContent("保存成功"));
            }

            //AddFrameworkToProject
            GUILayoutSplit("AddFrameworkToProject");
            _tool.Cfg.frameworkToProjectList = GUILayoutArray(_tool.Cfg.frameworkToProjectList);

            //AddFileToBuild
            GUILayoutSplit("AddFileToBuild");
            GUILayoutDictionary(_tool.Cfg.file2BuildList, "path", "projectPath");

            //SetBuildProperty
            GUILayoutSplit("SetBuildProperty");
            GUILayoutDictionary(_tool.Cfg.buildPropertyList, "name", "value");

            //AddPList
            GUILayoutSplit("AddPList");
            GUILayoutDictionary(_tool.Cfg.pListDataList, "key", "value");

            //AddUrlScheme
            GUILayoutSplit("AddUrlScheme");
            _tool.Cfg.urlSchemeList = GUILayoutArray(_tool.Cfg.urlSchemeList);

            //AddLSApplicationQueriesScheme
            GUILayoutSplit("AddLSApplicationQueriesScheme");
            _tool.Cfg.appQueriesSchemeList = GUILayoutArray(_tool.Cfg.appQueriesSchemeList);

            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();


        }
    }    
}