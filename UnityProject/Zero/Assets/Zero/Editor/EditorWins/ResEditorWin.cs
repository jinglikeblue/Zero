using Jing;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    public class ResEditorWin : AEditorWin
    {
        const string CONFIG_NAME = "ResCfg.json";

        public struct ConfigVO
        {
            /// <summary>
            /// 编译工具
            /// </summary>
            public string resDir;

            /// <summary>
            /// Manifest的AB文件资源名称
            /// </summary>
            public string manifestName;
        }

        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open()
        {
            var win = EditorWindow.GetWindow<ResEditorWin>();
            win.titleContent = new GUIContent("Res Manager");
            win.minSize = new Vector2(600, 300);
            win.maxSize = new Vector2(600, 300);
            win.Show();
        }

        ConfigVO cfg;
        
        private void OnEnable()
        {
            cfg = LoadConfig<ConfigVO>(CONFIG_NAME);
            if(null == cfg.resDir)
            {
                cfg.resDir = "." + Path.DirectorySeparatorChar;
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("保存配置"))
            {
                SaveConfig(cfg, CONFIG_NAME);
                ShowNotification(new GUIContent("保存成功"));
            }
            
            cfg.resDir = EditorGUILayout.TextField("Res目录:", cfg.resDir);

            GUILayout.Space(30);

            cfg.manifestName = EditorGUILayout.TextField("Manifest资源包名称:", cfg.manifestName);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("生成「res.json」"))
            {
                CreateResJsonFile();
            }
            EditorGUILayout.LabelField(ResJsonFilePath);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        string ResJsonFilePath
        {
            get
            {
                return Path.Combine(ResJsonFileDir, "res.json");
            }            
        }

        string ResJsonFileDir
        {
            get
            {
                return Path.Combine(cfg.resDir, EditorMenu.PlatformDirName);
            }
        }

        void CreateResJsonFile()
        {
            string filePath = ResJsonFilePath;
            if(File.Exists(filePath))
            {
                if (EditorUtility.DisplayDialog("警告！", "已存在文件「res.json」，是否覆盖？", "Yes", "No"))
                {
                    Create();
                }
            }
            else
            {
                Create();
            }
        }

        void Create()
        {
            string filePath = ResJsonFilePath;

            //首先删除旧的
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            

            ResVerVO res = new CreateResVO(ResJsonFileDir).Start();
            res.manifestName = cfg.manifestName;
            string jsonStr = LitJson.JsonMapper.ToJson(res);
            File.WriteAllText(filePath, jsonStr);
            
            //打开目录
            EditorMenu.OpenDirectory(ResJsonFileDir);
        }

        //AssetBundleManifest GetManifest()
        //{
        //    AssetBundle.UnloadAllAssetBundles(true);
        //    var file = FileSystem.CombinePaths(cfg.resDir, EditorMenu.PlatformDirName, cfg.manifestName);
        //    AssetBundle ab = AssetBundle.LoadFromFile(file);
        //    var name = typeof(AssetBundleManifest).Name;
        //    var manifest = ab.LoadAsset<AssetBundleManifest>(name);
        //    ab.Unload(false);
        //    return manifest;
        //}
    }
}