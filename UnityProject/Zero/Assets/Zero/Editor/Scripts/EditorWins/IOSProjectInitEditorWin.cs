using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    public class IOSProjectInitEditorWin : OdinEditorWindow
    {
        const string CONFIG_NAME = "ios_project_config.json";

        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open()
        {
            var win = GetWindow<IOSProjectInitEditorWin>("iOS 项目配置");
            win.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        }

        ConfigVO _cfg;

        public class ConfigVO
        {
            public string[] frameworkToProjectList = new string[0];

            public Dictionary<string, string> file2BuildList = new Dictionary<string, string>();

            public Dictionary<string, string> buildPropertyList = new Dictionary<string, string>();

            public Dictionary<string, string> pListDataList = new Dictionary<string, string>();

            public string[] urlSchemeList = new string[0];

            public string[] appQueriesSchemeList = new string[0];
        }

        override protected void OnEnable()
        {
            _cfg = EditorConfigUtil.LoadConfig<ConfigVO>(CONFIG_NAME);
            frameworkToProjectList = _cfg.frameworkToProjectList;
            file2BuildList = _cfg.file2BuildList;
            setBuildProperty = _cfg.buildPropertyList;
            addPListInfo = _cfg.pListDataList;
            urlSchemeList = _cfg.urlSchemeList;
            appQueriesSchemeList = _cfg.appQueriesSchemeList;
        }



        [LabelText("保存配置"), Button(size: ButtonSizes.Medium), PropertyOrder(-1)]
        void SaveConfig()
        {
            _cfg.frameworkToProjectList = frameworkToProjectList;
            _cfg.file2BuildList = file2BuildList;
            _cfg.buildPropertyList = setBuildProperty;
            _cfg.pListDataList = addPListInfo;
            _cfg.urlSchemeList = urlSchemeList;
            _cfg.appQueriesSchemeList = appQueriesSchemeList;


            EditorConfigUtil.SaveConfig(_cfg, CONFIG_NAME);
            ShowNotification(new GUIContent("保存成功"));
        }

        [Space(20)]
        [LabelText("Add Framework"), ListDrawerSettings(DraggableItems = false, NumberOfItemsPerPage = 5, Expanded = true)]
        public string[] frameworkToProjectList = new string[0];

        [Space(20)]
        [LabelText("Add Libs"), DictionaryDrawerSettings(KeyLabel = "Path", ValueLabel = "Project Path")]
        public Dictionary<string, string> file2BuildList = new Dictionary<string, string>();

        [Space(20)]
        [DictionaryDrawerSettings(KeyLabel = "Name", ValueLabel = "Value")]
        public Dictionary<string, string> setBuildProperty = new Dictionary<string, string>();

        [Space(20)]
        [DictionaryDrawerSettings(KeyLabel = "Key", ValueLabel = "Value")]
        public Dictionary<string, string> addPListInfo = new Dictionary<string, string>();

        [Space(20)]
        [LabelText("UrlScheme"), ListDrawerSettings(DraggableItems = false, NumberOfItemsPerPage = 5, Expanded = true)]
        public string[] urlSchemeList = new string[0];

        [Space(20)]
        [LabelText("LSApplicationQueriesScheme"), ListDrawerSettings(DraggableItems = false, NumberOfItemsPerPage = 5, Expanded = true)]
        public string[] appQueriesSchemeList = new string[0];
    }
}