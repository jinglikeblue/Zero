using Jing;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    /// <summary>
    /// 配置文件窗口
    /// </summary>
    public class SettingEditorWin : OdinEditorWindow
    {
        const string CONFIG_NAME = "setting_config.json";

        /// <summary>
        /// 设置数据
        /// </summary>
        SettingVO cfg;

        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open()
        {
            var win = EditorWindow.GetWindow<SettingEditorWin>();
            win.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            SetCfg(EditorConfigUtil.LoadConfig<SettingVO>(CONFIG_NAME));
        }

        void SetCfg(SettingVO cfg)
        {
            this.cfg = cfg;
            version = cfg.client.version;
            url = cfg.client.url;
            type = cfg.client.type;
            netResRoot = cfg.netResRoot;
            startupResGroups = cfg.startupResGroups;
            startupParams = cfg.startupParams;
        }

        void UpdateCfg()
        {
            cfg.client.version = version;
            cfg.client.url = url;
            cfg.client.type = type;
            cfg.netResRoot = netResRoot;
            cfg.startupResGroups = startupResGroups;
            cfg.startupParams = startupParams;
        }

        [Button("保存配置", buttonSize: ButtonSizes.Medium),PropertyOrder(-1)]
        void SaveConfig()
        {
            UpdateCfg();
            EditorConfigUtil.SaveConfig(cfg, CONFIG_NAME);
        }

        [Button("读取「setting.json」", buttonSize: ButtonSizes.Medium), PropertyOrder(-1)]
        void LoadExistSettingJson()
        {
            var selectedFile = EditorUtility.OpenFilePanel("选择文件", Application.dataPath, "json");
            if (false == string.IsNullOrEmpty(selectedFile))
            {
                try
                {
                    var jsonStr = File.ReadAllText(selectedFile);
                    SetCfg(LitJson.JsonMapper.ToObject<SettingVO>(jsonStr));
                }
                catch(Exception e)
                {
                    Debug.LogError("读取选择的setting.json文件失败：" + selectedFile);
                    Debug.LogError(e);
                }
            }
        }

        [Title("客户端版本")]
        [LabelText("版本号")]
        public string version;

        [LabelText("更新地址URL")]
        public string url;

        [LabelText("更新方式"), ValueDropdown("ClientUpdateType")]
        public int type;

        private static IEnumerable ClientUpdateType = new ValueDropdownList<int>()
        {
            { "安装包更新", 0 },
            { "网页更新", 1 }
        };

        [Title("远端资源")]
        [LabelText("远端资源根目录URL：")]
        public string netResRoot;

        [LabelText("启动资源组"),ListDrawerSettings(NumberOfItemsPerPage = 7, Expanded = false)]
        public string[] startupResGroups;

        [Title("启动参数配置")]        
        public Dictionary<string, string> startupParams;


        [Button("发布「setting.json」", buttonSize:ButtonSizes.Medium), PropertyOrder(999)]
        void BuildSettingJsonFile()
        {            
            if (false == Directory.Exists(ZeroEditorConst.PUBLISH_RES_ROOT_DIR))
            {
                Directory.CreateDirectory(ZeroEditorConst.PUBLISH_RES_ROOT_DIR);
            }

            var filePath = FileSystem.CombinePaths(ZeroEditorConst.PUBLISH_RES_ROOT_DIR, ZeroConst.SETTING_FILE_NAME);
            if (File.Exists(filePath) && false == EditorUtility.DisplayDialog("警告！", "已存在文件「setting.json」，是否覆盖？", "Yes", "No"))
            {
                return;
            }

            UpdateCfg();
            string jsonStr = LitJson.JsonMapper.ToPrettyJson(cfg);
            File.WriteAllText(filePath, jsonStr);

            //打开目录
            ZeroEditorUtil.OpenDirectory(ZeroEditorConst.PUBLISH_RES_ROOT_DIR);
        }
    }
}