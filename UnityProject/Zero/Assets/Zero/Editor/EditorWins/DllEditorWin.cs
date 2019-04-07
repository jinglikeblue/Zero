using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    public class DllEditorWin : AEditorWin
    {
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

        DllPublishConfigModel _cfg;

        private void OnEnable()
        {
            _cfg = new DllPublishConfigModel();
        }        

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            if (GUILayout.Button("保存配置"))
            {
                _cfg.Save();                
                ShowNotification(new GUIContent("保存成功"));
            }
            _cfg.VO.resDir = EditorGUILayout.TextField("Res目录:", _cfg.VO.resDir);
            GUILayout.Space(10);

            _cfg.VO.ilDevelopDir = EditorGUILayout.TextField("DLL开发目录:", _cfg.VO.ilDevelopDir);           
           

            GUILayout.BeginHorizontal();

            _cfg.VO.ilProjDir = EditorGUILayout.TextField("DLL项目目录:", _cfg.VO.ilProjDir);

            if(GUILayout.Button("清空项目目录",GUILayout.Width(100)))
            {
                if (EditorUtility.DisplayDialog("警告！", "是否确认清空项目目录", "Yes", "No"))
                {
                    string projCodeDir = Path.Combine(_cfg.VO.ilProjDir, "codes");
                    if (Directory.Exists(projCodeDir))
                    {
                        Directory.Delete(projCodeDir, true);
                    }
                    EditorUtility.DisplayDialog("Success", "操作完成", "Ok", null);
                }
            }

            GUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("是否在发布DLL的时候自动拷贝代码", GUILayout.Width(200));

            _cfg.VO.isAudoCopy = EditorGUILayout.Toggle(_cfg.VO.isAudoCopy);

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
            _cfg.VO.devenvPath = EditorGUILayout.TextField("Devenv地址:", _cfg.VO.devenvPath);
            _cfg.VO.ilProjPath = EditorGUILayout.TextField("DLL项目csproj文件路径：", _cfg.VO.ilProjPath);

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
            string projCodeDir = Path.Combine(_cfg.VO.ilProjDir, "codes");

            if (Directory.Exists(_cfg.VO.ilDevelopDir))
            {
                if (Directory.Exists(projCodeDir))
                {
                    Directory.Delete(projCodeDir, true);
                }                
                FileUtil.CopyFileOrDirectory(_cfg.VO.ilDevelopDir, projCodeDir);
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
            string projCodeDir = Path.Combine(_cfg.VO.ilProjDir, "codes");

            if (Directory.Exists(_cfg.VO.ilDevelopDir))
            {
                if(Directory.Exists(projCodeDir))
                {
                    Directory.Delete(projCodeDir,true);
                }
                FileUtil.MoveFileOrDirectory(_cfg.VO.ilDevelopDir, projCodeDir);
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
            string projCodeDir = Path.Combine(_cfg.VO.ilProjDir, "codes");

            if (Directory.Exists(projCodeDir))
            {
                if (Directory.Exists(_cfg.VO.ilDevelopDir))
                {
                    Directory.Delete(_cfg.VO.ilDevelopDir, true);
                }
                FileUtil.MoveFileOrDirectory(projCodeDir, _cfg.VO.ilDevelopDir);
                AssetDatabase.Refresh();
            }
            else
            {
                ShowNotification(new GUIContent("文件夹不存在"));
            }
        }

        private void OpenCsproj()
        {
            EditorMenu.OpenDirectory(Path.GetDirectoryName(_cfg.VO.ilProjPath));
            //Process p = new Process();
            //p.StartInfo.FileName = _cfg.VO.ilProjPath;
            //p.Start();
        }

        private void ReleaseDLL()
        {
            if (_cfg.VO.isAudoCopy)
            {
                Copy2DllProj();
            }
            
            var cmd = new DllPublishCommand();
            cmd.onComplete += OnPublishDllComplete;
            cmd.Execute();                 
        }

        private void OnPublishDllComplete(string releaseDir)
        {
            EditorMenu.OpenDirectory(releaseDir);
            UnityEngine.Debug.Log("dll release success");           
        }
    }
}