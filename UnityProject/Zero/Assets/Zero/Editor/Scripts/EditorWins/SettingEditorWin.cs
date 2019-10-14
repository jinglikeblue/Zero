using Jing;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    /// <summary>
    /// 配置文件窗口
    /// </summary>
    public class SettingEditorWin : AEditorWin
    {
        const string CONFIG_NAME = "SettingCfg.json";

        public struct ConfigVO
        {
            /// <summary>
            /// 保存路径
            /// </summary>
            public string saveDir;

            /// <summary>
            /// 设置数据
            /// </summary>
            public SettingVO data;
        }

        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open()
        {
            var win = EditorWindow.GetWindow<SettingEditorWin>();
            win.titleContent = new GUIContent("Setting Config");
            win.minSize = new Vector2(1000, 800);
            win.maxSize = new Vector2(1280, 800);
            win.Show();
        }

        ConfigVO cfg;
        Vector2 _pos = Vector2.zero;
        GUIDictionary _startParamsDic = new GUIDictionary();
        GUIDictionary _settingJumpDic = new GUIDictionary();

        private void OnEnable()
        {
            cfg = EditorConfigUtil.LoadConfig<ConfigVO>(CONFIG_NAME);

            if (cfg.data.startupResGroups == null)
            {
                cfg.data.startupResGroups = new string[0];
            }

            if (cfg.data.startupParams == null)
            {
                cfg.data.startupParams = new Dictionary<string, string>();
            }

            _startParamsDic.SetData(cfg.data.startupParams, "Key", "Value");                     
        }

        private void OnGUI()
        {
            bool isMouseDown = Event.current.type == EventType.MouseDown ? true : false;

            EditorGUILayout.BeginVertical();

            GUIText.LayoutHead("保存setting.json");            

            if (GUILayout.Button("保存配置", GUILayout.Width(200)))
            {
                EditorConfigUtil.SaveConfig(cfg, CONFIG_NAME);
                ShowNotification(new GUIContent("保存成功"));
                _startParamsDic.Reload();
                _settingJumpDic.Reload();
            }
            cfg.saveDir = EditorGUILayout.TextField("Setting文件保存目录:", cfg.saveDir);
            EditorGUILayout.Space();


            _pos = GUILayout.BeginScrollView(_pos);

            GUIText.LayoutHead("setting.json编辑");

            GUIText.LayoutSplit("客户端版本");
            EditorGUILayout.BeginHorizontal();
            cfg.data.client.version = EditorGUILayout.TextField("客户端版本号:", cfg.data.client.version, GUILayout.Width(300));
            if (GUILayout.Button("获取当前版本号", GUILayout.Width(100)))
            {
                cfg.data.client.version = Application.version;
            }

            GUILayout.Space(20);
            EditorGUILayout.LabelField("更新方式：", GUILayout.Width(60));

            cfg.data.client.type = EditorGUILayout.Popup(cfg.data.client.type, new string[] { "安装包更新", "网页更新" }, GUILayout.Width(150));

            EditorGUILayout.EndHorizontal();

            cfg.data.client.url = EditorGUILayout.TextField("客户端URl:", cfg.data.client.url);
            

            //--------------------资源配置
            GUIText.LayoutSplit("联网资源");
            cfg.data.netResRoot = EditorGUILayout.TextField("网络资源目录:", cfg.data.netResRoot);

            EditorGUILayout.LabelField("启动资源组列表");
            int groupSize = EditorGUILayout.IntField("数量:", cfg.data.startupResGroups.Length, GUILayout.MaxWidth(200));
            if (groupSize != cfg.data.startupResGroups.Length)
            {
                string[] newArr = new string[groupSize];
                Array.Copy(cfg.data.startupResGroups, 0, newArr, 0, groupSize < cfg.data.startupResGroups.Length ? groupSize : cfg.data.startupResGroups.Length);
                cfg.data.startupResGroups = newArr;
            }

            for (int i = 0; i < cfg.data.startupResGroups.Length; i++)
            {
                cfg.data.startupResGroups[i] = EditorGUILayout.TextField("资源组名称:", cfg.data.startupResGroups[i]);
            }

            //-------------------------------
            GUIText.LayoutSplit("额外参数");
            EditorGUILayout.LabelField("配置文件附带参数");
            cfg.data.startupParams = _startParamsDic.OnGUI(isMouseDown);
            //GUILayoutDictionary(cfg.data.startupParams, "Key", "Value");
            GUILayout.EndScrollView();

            //----------------------------------------------------
            GUILayout.Space(20);
            if (GUILayout.Button("生成[Setting.json]"))
            {
                CreateSettingJsonFile();
                _startParamsDic.Reload();
                _settingJumpDic.Reload();
            }           

            EditorGUILayout.EndVertical();
        }

        string SavePath
        {
            get
            {
                var dir = FileSystem.CombinePaths(cfg.saveDir, ZeroConst.PLATFORM_DIR_NAME);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                return FileSystem.CombinePaths(dir, "setting.json");
            }
        }

        void CreateSettingJsonFile()
        {
            var sp = SavePath;
            if (File.Exists(sp) && false == EditorUtility.DisplayDialog("警告！", "已存在文件「setting.json」，是否覆盖？", "Yes", "No"))
            {
                return;
            }

            string jsonStr = LitJson.JsonMapper.ToJson(cfg.data);
            File.WriteAllText(sp, jsonStr);

            //打开目录
            ZeroEditorUtil.OpenDirectory(Path.GetDirectoryName(sp));
        }
    }
}