using Jing;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace ZeroEditor
{
    class IOSProjectInitModule : AEditorModule
    {
        public const string CONFIG_NAME = "ios_project_config.json";

        IOSProjectInitConfigVO _cfg;

        public IOSProjectInitModule(EditorWindow editorWin) : base(editorWin)
        {
            _cfg = EditorConfigUtil.LoadConfig<IOSProjectInitConfigVO>(CONFIG_NAME);
            if(null == _cfg)
            {
                _cfg = new IOSProjectInitConfigVO();
            }
            frameworkToProjectList = _cfg.frameworkToProjectList;
            file2BuildList = _cfg.file2BuildList;
            setBuildProperty = _cfg.buildPropertyList;
            addPListInfo = _cfg.pListDataList;
            urlSchemeList = _cfg.urlSchemeList;
            appQueriesSchemeList = _cfg.appQueriesSchemeList;
        }

        [Title("Unity打包的xCode项目自动化配置",titleAlignment:TitleAlignments.Centered)]
        [LabelText("保存配置"), Button(size: ButtonSizes.Large), PropertyOrder(-1)]
        void SaveConfig()
        {
            _cfg.frameworkToProjectList = frameworkToProjectList;
            _cfg.file2BuildList = file2BuildList;
            _cfg.buildPropertyList = setBuildProperty;
            _cfg.pListDataList = addPListInfo;
            _cfg.urlSchemeList = urlSchemeList;
            _cfg.appQueriesSchemeList = appQueriesSchemeList;


            EditorConfigUtil.SaveConfig(_cfg, CONFIG_NAME);
            editorWin.ShowTip("保存成功!");            
        }

        [Space(20)]
        [LabelText("Add Framework"), ListDrawerSettings(DraggableItems = false, NumberOfItemsPerPage = 5, Expanded = true)]
        public string[] frameworkToProjectList;

        [Space(20)]
        [ShowInInspector]
        [LabelText("Add Libs"), DictionaryDrawerSettings(KeyLabel = "Path", ValueLabel = "Project Path")]
        public Dictionary<string, string> file2BuildList;

        [Space(20)]
        [ShowInInspector]
        [DictionaryDrawerSettings(KeyLabel = "Name", ValueLabel = "Value")]
        public Dictionary<string, string> setBuildProperty;

        [Space(20)]
        [ShowInInspector]
        [DictionaryDrawerSettings(KeyLabel = "Key", ValueLabel = "Value")]
        public Dictionary<string, string> addPListInfo;

        [Space(20)]
        [LabelText("UrlScheme"), ListDrawerSettings(DraggableItems = false, NumberOfItemsPerPage = 5, Expanded = true)]
        public string[] urlSchemeList;

        [Space(20)]
        [LabelText("LSApplicationQueriesScheme"), ListDrawerSettings(DraggableItems = false, NumberOfItemsPerPage = 5, Expanded = true)]
        public string[] appQueriesSchemeList;
    }
}
