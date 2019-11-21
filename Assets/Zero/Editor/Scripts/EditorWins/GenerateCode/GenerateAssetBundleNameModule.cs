using Jing;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    public class GenerateAssetBundleNameModule : AEditorModule
    {
        /// <summary>
        /// 配置文件位置
        /// </summary>
        public const string CONFIG_NAME = "asset_bundle_name_config.json";

        public struct ConfigVO
        {
            public List<AssetBundleItemVO> abList;
            public string viewClassNS;
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <returns></returns>
        public static ConfigVO LoadConfig()
        {
            var cfg = EditorConfigUtil.LoadConfig<ConfigVO>(CONFIG_NAME);
            return cfg;
        }        

        ConfigVO cfg;        

        public GenerateAssetBundleNameModule(EditorWindow win) : base(win)
        {
            _isFindingAB = true;
            var findCmd = new FindAssetBundlesCommand();
            findCmd.onFinished += OnFindFinished;
            findCmd.Excute();            
        }

        private void OnFindFinished(FindAssetBundlesCommand cmd, List<AssetBundleItemVO> list)
        {
            abList = list;
            cfg = cmd.cfg;
            viewClassNS = cfg.viewClassNS;
            _isFindingAB = false;
        }

        [HideLabel, DisplayAsString, ShowIf("_isFindingAB")]
        public string findTip = "正在同步AssetBundle数据......";

        bool _isFindingAB = true;

        [Title("资源相关代码生成", TitleAlignment = TitleAlignments.Centered)]
        [LabelText("保存配置"), Button(size: ButtonSizes.Large), PropertyOrder(-1)]
        void SaveConfig()
        {
            cfg.viewClassNS = viewClassNS;
            cfg.abList = abList;
            EditorConfigUtil.SaveConfig(cfg, CONFIG_NAME);
            //this.ShowTip("保存完毕");
        }

        [PropertySpace(10)]
        [LabelText("开始生成"), Button(size: ButtonSizes.Large), PropertyOrder(-1)]
        void GeneratedAssetBundleNameClass()
        {
            new GenerateABClassCommand(abList).Excute();
            new GenerateAutoViewRegisterClassCommand(abList, viewClassNS).Excute();
            editorWin.ShowTip("生成完毕!");
        }

        [Space(10)]
        [LabelText("资源名称类生成地址")]
        [DisplayAsString]
        [InlineButton("OpenABClassFile", "Open")]
        public string abClassPath = GenerateABClassCommand.OUTPUT_FILE;

        [LabelText("视图类命名空间前缀(以\".\"结尾)")]
        public string viewClassNS;

        [LabelText("视图注册类生成地址")]
        [DisplayAsString]
        [InlineButton("OpenRegisterClassFile", "Open")]
        public string registerClassPath = GenerateAutoViewRegisterClassCommand.OUTPUT_FILE;

        void OpenABClassFile()
        {
            OpenFile(GenerateABClassCommand.OUTPUT_FILE);
        }

        void OpenRegisterClassFile()
        {
            OpenFile(GenerateAutoViewRegisterClassCommand.OUTPUT_FILE);
        }

        void OpenFile(string file)
        {
            if (File.Exists(file))
            {
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(file, 0);
            }
            else
            {
                editorWin.ShowTip("文件不存在!");
            }
        }

        [Space(10)]
        [ShowInInspector]
        [LabelText("AssetBundle List"), ListDrawerSettings(IsReadOnly = true, Expanded = true, NumberOfItemsPerPage = 6), HideIf("_isFindingAB")]
        public List<AssetBundleItemVO> abList;
    }
}
