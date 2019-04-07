using Jing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Zero.Edit
{
    public class ABEditorWin : AEditorWin
    {
        const string CONFIG_NAME = "AeesetBundleCfg.json";

        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {

        }

        [PostProcessScene]
        public static void OnPostProcessScene()
        {

        }



        public static void MoveBackHotResDir()
        {
            Debug.Log("MoveHotResDirBack");
            var cfgVO = EditorConfigUtil.LoadConfig<ConfigVO>(CONFIG_NAME);
            if (cfgVO.isBestResourcesDir == false || cfgVO.selectedHotResDir == null)
            {
                return;
            }

            var di = new DirectoryInfo(Application.dataPath);
            var sourceDir = FileSystem.CombineDirs(false, di.Parent.FullName, "TempZero", cfgVO.selectedHotResDir);
            if (false == Directory.Exists(sourceDir))
            {
                return;
            }

            var targetDir = FileSystem.CombineDirs(false, Application.dataPath, cfgVO.scanStartDir, cfgVO.selectedHotResDir);


            FileUtil.MoveFileOrDirectory(sourceDir, targetDir);

            AssetDatabase.Refresh();
        }
        
        public static void MoveOutHotResDir()
        {
            Debug.Log("MoveHotResDirOut");

            var cfgVO = EditorConfigUtil.LoadConfig<ConfigVO>(CONFIG_NAME);
            if(cfgVO.isBestResourcesDir == false || cfgVO.selectedHotResDir == null)
            {
                return;
            }
            var sourceDir = FileSystem.CombineDirs(false, Application.dataPath, cfgVO.scanStartDir, cfgVO.selectedHotResDir);
            if(false == Directory.Exists(sourceDir))
            {
                return;
            }
            var di = new DirectoryInfo(Application.dataPath);
            var targetDir = FileSystem.CombineDirs(false, di.Parent.FullName, "TempZero", cfgVO.selectedHotResDir);
            if (Directory.Exists(targetDir))
            {
                //如果目标目录存在，则删除
                Directory.Delete(targetDir, true);
            }

            var targetParent = Path.GetDirectoryName(targetDir);
            if(false == Directory.Exists(targetParent))
            {
                Directory.CreateDirectory(targetParent);
            }
            FileUtil.MoveFileOrDirectory(sourceDir, targetDir);

            AssetDatabase.Refresh();
        }


        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open()
        {
            var win = EditorWindow.GetWindow<ABEditorWin>();
            win.titleContent = new GUIContent("AB Manager");
            win.minSize = new Vector2(800, 500);
            win.maxSize = new Vector2(1000, 500);
            win.Show();            
        }

        

        public struct ConfigVO
        {
            /// <summary>
            /// 发布目录
            /// </summary>
            public string resDir;

            /// <summary>
            /// 是否删除Manifest文件
            /// </summary>
            public bool isKeepManifestFile;

            /// <summary>
            /// 发布模式
            /// </summary>
            public int publishMode;

            /// <summary>
            /// 发布后是否打开文件夹
            /// </summary>
            public bool isOpenDirOnPublishComplete;

            /// <summary>
            /// 是否使用最好的资源目录模式，兼容Resources读取以及AssetBundle读取的目录结构
            /// </summary>
            public bool isBestResourcesDir;

            /// <summary>
            /// 最佳资源模式下选中的目录
            /// </summary>
            public string selectedHotResDir;

            /// <summary>
            /// AssetBundle资源扫描起始目录(AB文件以资源所在文件夹与该目录的相对路径命名)
            /// </summary>
            public string scanStartDir;
        }

        ConfigVO cfg;

        private void OnEnable()
        {            
            cfg = EditorConfigUtil.LoadConfig<ConfigVO>(CONFIG_NAME);
            if (null == cfg.resDir)
            {
                cfg.resDir = "." + Path.DirectorySeparatorChar;
            }
        }        

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("保存配置"))
            {
                EditorConfigUtil.SaveConfig(cfg, CONFIG_NAME);
                ShowNotification(new GUIContent("保存成功"));
            }

            cfg.resDir = EditorGUILayout.TextField("Res目录:", cfg.resDir);
            EditorGUILayout.LabelField("目标目录：" + AssetBundleDir);
            GUILayout.Space(20);
            //EditorGUILayout.BeginHorizontal();

            cfg.isKeepManifestFile = GUILayout.Toggle(cfg.isKeepManifestFile, "保留「.manifest」文件", GUILayout.Width(150));

            cfg.publishMode = EditorGUILayout.Popup(cfg.publishMode, new string[] { "增量发布", "全量发布" }, GUILayout.Width(150));

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(50);
            cfg.isBestResourcesDir = EditorGUILayout.Toggle(cfg.isBestResourcesDir, GUILayout.Width(20));
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("(推荐)使用最好的资源目录模式，兼容Resources读取以及AssetBundle读取的目录结构。");
            EditorGUILayout.LabelField("注意：该模式下扫描的目录锁定为「Assets/Resources」目录（才能同时兼容INLINE以及NET两种运行模式）");
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            if (cfg.isBestResourcesDir)
            {
                cfg.scanStartDir = "Resources";
                EditorGUILayout.LabelField("Resources目录下作为热更资源的目录（在发布项目时请记得「排除」，发布后请记得「恢复」）");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Build项目时需要排除的Res目录: Assets/Resources/", GUILayout.Width(300));

                if(null == cfg.selectedHotResDir)
                {
                    cfg.selectedHotResDir = "";
                }

                cfg.selectedHotResDir = EditorGUILayout.TextField("", cfg.selectedHotResDir);

               

                //string resourcesDir = Application.dataPath + "/Resources";
                
                //string[] dirs = Directory.GetDirectories(resourcesDir);
                
                //var moveOutDir = FileSystem.CombineDirs(false, Path.GetDirectoryName(Application.dataPath), "TempZero", cfg.selectedHotResDir);
                //if(Directory.Exists(moveOutDir))
                //{
                //    string[] newDirs = new string[dirs.Length + 1];
                //    Array.Copy(dirs,0, newDirs,1, dirs.Length);
                //    newDirs[0] = moveOutDir;
                //    dirs = newDirs;
                //}

                //if (dirs.Length > 0)
                //{
                //    int idx = 0;
                //    for (int i = 0; i < dirs.Length; i++)
                //    {
                //        var di = new DirectoryInfo(dirs[i]);
                //        string dir = di.Name;
                //        dirs[i] = dir;
                //        if (dirs[i] == cfg.selectedHotResDir)
                //        {
                //            idx = i;
                //        }
                //    }

                //    idx = EditorGUILayout.Popup(idx, dirs);
                //    cfg.selectedHotResDir = dirs[idx];
                //}
                

                var sourceDir = FileSystem.CombineDirs(false, Application.dataPath, cfg.scanStartDir, cfg.selectedHotResDir);
                if (Directory.Exists(sourceDir))
                {
                    if (GUILayout.Button("排除"))
                    {
                        MoveOutHotResDir();                        
                    }
                }
                else
                {
                    if (GUILayout.Button("恢复"))
                    {
                        MoveBackHotResDir();                        
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            

            EditorGUILayout.LabelField("AssetBundle资源扫描起始目录(AB文件以资源所在文件夹与该目录的相对路径命名):");
            if (false == cfg.isBestResourcesDir)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Assets/", GUILayout.Width(50));
                //GUI.SetNextControlName("InputScanStartDir");
                cfg.scanStartDir = EditorGUILayout.TextField("", cfg.scanStartDir);
                GUILayout.Space(20);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.LabelField(string.Format("扫描目录：{0}", ScanStartDir));

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("发布AB", GUILayout.Width(200)))
            {
                BuildAssetBundles();
            }

            cfg.isOpenDirOnPublishComplete = GUILayout.Toggle(cfg.isOpenDirOnPublishComplete, "完成后打开发布目录", GUILayout.Width(150));

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20);

            //EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        string ScanStartDir
        {
            get
            {
                string dir = string.Format("Assets/{0}/", cfg.scanStartDir);
                do
                {
                    dir = dir.Replace("//", "/");
                }
                while (dir.IndexOf("//") > -1);

                return dir;
            }
        }

        string AssetBundleDir
        {
            get
            {
                string dir = FileSystem.CombineDirs(false, cfg.resDir, ZeroEditorUtil.PlatformDirName, ZeroEditorUtil.AB_DIR);
                return dir;
            }
        }

        void BuildAssetBundles()
        {
            if (cfg.publishMode == 1)
            {
                if (Directory.Exists(AssetBundleDir))
                {
                    //删除目标目录
                    Directory.Delete(AssetBundleDir, true);
                }
            }
            new AssetBundleBuilder(AssetBundleDir, ScanStartDir).Start();

            if(false == cfg.isKeepManifestFile)
            {
                FileSystem.DeleteFilesByExt(AssetBundleDir, ".manifest");
            }

            if (cfg.isOpenDirOnPublishComplete)
            {
                ZeroEditorUtil.OpenDirectory(AssetBundleDir);
            }
        }
    }
}