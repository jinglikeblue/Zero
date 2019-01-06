using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    public class DllEditorWin : AEditorWin
    {
        const string CONFIG_NAME = "DllCfg.json";

        public struct ConfigVO
        {
            /// <summary>
            /// 编译工具
            /// </summary>
            public string devenvPath;

            /// <summary>
            /// 开发目录
            /// </summary>
            public string ilDevelopDir;

            /// <summary>
            /// 项目目录
            /// </summary>
            public string ilProjDir;

            /// <summary>
            /// 项目csproj路径
            /// </summary>
            public string ilProjPath;

            /// <summary>
            /// 是否在发布DLL的时候自动拷贝代码
            /// </summary>
            public bool isAudoCopy;
        }

        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open()
        {
            var win = EditorWindow.GetWindow<DllEditorWin>();
            win.titleContent = new GUIContent("Dll Manager");            
            win.minSize = new Vector2(1000, 300);
            win.maxSize = new Vector2(600, 300);
            win.Show();
        }

        ConfigVO cfg;

        private void OnEnable()
        {
            cfg = LoadConfig<ConfigVO>(CONFIG_NAME);
        }        

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            if (GUILayout.Button("保存配置"))
            {
                SaveConfig(cfg, CONFIG_NAME);
                ShowNotification(new GUIContent("保存成功"));
            }

            GUILayout.Space(10);          
            
            cfg.ilDevelopDir = EditorGUILayout.TextField("DLL开发目录:", cfg.ilDevelopDir);           
           

            GUILayout.BeginHorizontal();
            
            cfg.ilProjDir = EditorGUILayout.TextField("DLL项目目录:", cfg.ilProjDir);

            if(GUILayout.Button("清空项目目录",GUILayout.Width(100)))
            {
                string projCodeDir = Path.Combine(cfg.ilProjDir, "codes");
                if (Directory.Exists(projCodeDir))
                {
                    Directory.Delete(projCodeDir, true);
                }  
                EditorUtility.DisplayDialog("Success","操作完成","Ok",null);
            }

            GUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("是否在发布DLL的时候自动拷贝代码", GUILayout.Width(200));

            cfg.isAudoCopy = EditorGUILayout.Toggle(cfg.isAudoCopy);

            EditorGUILayout.EndHorizontal();

            

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("开发目录 ->(Copy) 项目目录"))
            {
                if (EditorUtility.DisplayDialog("警告！", "是否确认执行(目标目录将被覆盖)", "Yes", "No"))
                {
                    Copy2DllProj();
                }
            }

            

            //if (GUILayout.Button("开发目录 -> 项目目录"))
            //{
            //    if(EditorUtility.DisplayDialog("警告！", "是否确认执行(请确认已SVN备份，移动时会删除目标目录)", "Yes","No"))
            //    {
            //        Develop2Publish();
            //    }
            //}

            //if (GUILayout.Button("项目目录 -> 开发目录"))
            //{
            //    if (EditorUtility.DisplayDialog("警告！", "是否确认执行(请确认已SVN备份，移动时会删除目标目录)", "Yes", "No"))
            //    {
            //        Publish2Develop();
            //    }
            //}

            EditorGUILayout.EndHorizontal();




            EditorGUILayout.Space();
            cfg.devenvPath = EditorGUILayout.TextField("Devenv地址:", cfg.devenvPath);
            cfg.ilProjPath = EditorGUILayout.TextField("DLL项目csproj文件路径：", cfg.ilProjPath);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("VS中打开DLL项目", GUILayout.Width(200)))
            {
                OpenCsproj();
            }
            if (GUILayout.Button("发布DLL"))
            {                
                ReleaseDLL();
            }
            GUILayout.EndHorizontal();


            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 拷贝代码到Proj项目
        /// </summary>
        private void Copy2DllProj()
        {
            string projCodeDir = Path.Combine(cfg.ilProjDir, "codes");

            if (Directory.Exists(cfg.ilDevelopDir))
            {
                if (Directory.Exists(projCodeDir))
                {
                    Directory.Delete(projCodeDir, true);
                }                
                FileUtil.CopyFileOrDirectory(cfg.ilDevelopDir, projCodeDir);
                Jing.FileSystem.DeleteFilesByExt(projCodeDir, "meta");

                AssetDatabase.Refresh();
            }
            else
            {
                ShowNotification(new GUIContent("文件夹不存在"));
            }
        }

        private void Develop2Publish()
        {
            string projCodeDir = Path.Combine(cfg.ilProjDir, "codes");

            if (Directory.Exists(cfg.ilDevelopDir))
            {
                if(Directory.Exists(projCodeDir))
                {
                    Directory.Delete(projCodeDir,true);
                }
                FileUtil.MoveFileOrDirectory(cfg.ilDevelopDir, projCodeDir);
                Jing.FileSystem.DeleteFilesByExt(projCodeDir, "meta");

                AssetDatabase.Refresh();
            }
            else
            {
                ShowNotification(new GUIContent("文件夹不存在"));
            }
        }

        private void Publish2Develop()
        {
            string projCodeDir = Path.Combine(cfg.ilProjDir, "codes");

            if (Directory.Exists(projCodeDir))
            {
                if (Directory.Exists(cfg.ilDevelopDir))
                {
                    Directory.Delete(cfg.ilDevelopDir, true);
                }
                FileUtil.MoveFileOrDirectory(projCodeDir, cfg.ilDevelopDir);
                AssetDatabase.Refresh();
            }
            else
            {
                ShowNotification(new GUIContent("文件夹不存在"));
            }
        }

        private void OpenCsproj()
        {
            EditorMenu.OpenDirectory(Path.GetDirectoryName(cfg.ilProjPath));
            //Process p = new Process();
            //p.StartInfo.FileName = cfg.ilProjPath;
            //p.Start();
        }

        private void ReleaseDLL()
        {
            if (cfg.isAudoCopy)
            {
                Copy2DllProj();
            }

            Process p = new Process();
            p.StartInfo.FileName = cfg.devenvPath;// @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\devenv";  //确定程序名
            p.StartInfo.Arguments = string.Format("\"{0}\" /Rebuild \"Release|AnyCPU\"", cfg.ilProjPath);// @"""E:\projects\unity\Zero\UnityProject\ZeroIL\ZeroIL\ZeroIL.csproj"" /build";  //指定程式命令行
            //p.StartInfo.UseShellExecute = false;   //是否使用Shell
            //p.StartInfo.RedirectStandardInput = true;   //重定向输入
            //p.StartInfo.RedirectStandardOutput = true;   //重定向输出
            //p.StartInfo.RedirectStandardError = true;
            //p.StartInfo.RedirectStandardError = true;    //重定向输出错误
            //p.StartInfo.CreateNoWindow = true;        //设置不显示窗口
            p.Start();
        }
    }
}