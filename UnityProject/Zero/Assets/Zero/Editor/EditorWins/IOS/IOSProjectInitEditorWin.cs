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
            win.maxSize = new Vector2(1000, 1000);
            win.Show();
        }

        IOSProjectInitConfig _tool;
        Vector2 _pos = Vector2.zero;

        GUIDictionary _file2BuildDic = new GUIDictionary();
        GUIDictionary _buildPropertyDic = new GUIDictionary();
        GUIDictionary _pListDataDic = new GUIDictionary();

        private void OnEnable()
        {
            _tool = new IOSProjectInitConfig();

            _file2BuildDic.SetData(_tool.Cfg.file2BuildList, "path", "projectPath");
            _buildPropertyDic.SetData(_tool.Cfg.buildPropertyList, "name", "value");
            _pListDataDic.SetData(_tool.Cfg.pListDataList, "key", "value");
        }




        private void OnGUI()
        {
            bool isMouseDown = Event.current.type == EventType.MouseDown ? true : false;

            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("保存配置"))
            {
                _tool.SaveCfg();
                ShowNotification(new GUIContent("保存成功"));
                _file2BuildDic.Reload();
                _buildPropertyDic.Reload();
                _pListDataDic.Reload();
            }

            _pos = GUILayout.BeginScrollView(_pos);

            //AddFrameworkToProject
            GUILayoutSplit("AddFrameworkToProject");
            _tool.Cfg.frameworkToProjectList = GUILayoutArray(_tool.Cfg.frameworkToProjectList);

            //AddFileToBuild
            GUILayoutSplit("AddFileToBuild");
            _file2BuildDic.OnGUI(isMouseDown);

            //SetBuildProperty
            GUILayoutSplit("SetBuildProperty");
            _buildPropertyDic.OnGUI(isMouseDown);            

            //AddPList
            GUILayoutSplit("AddPList");
            _pListDataDic.OnGUI(isMouseDown);            

            //AddUrlScheme
            GUILayoutSplit("AddUrlScheme");
            _tool.Cfg.urlSchemeList = GUILayoutArray(_tool.Cfg.urlSchemeList);

            //AddLSApplicationQueriesScheme
            GUILayoutSplit("AddLSApplicationQueriesScheme");
            _tool.Cfg.appQueriesSchemeList = GUILayoutArray(_tool.Cfg.appQueriesSchemeList);

            EditorGUILayout.Space();

            GUILayout.EndScrollView();

            EditorGUILayout.EndVertical();


        }
    }    
}