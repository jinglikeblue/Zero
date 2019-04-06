using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    public class ILRuntimeEditorWin : AEditorWin
    {
        const string CONFIG_NAME = "ILRuntimeCfg.json";

        public class ConfigVO
        {
            public string bindingCodeDir = "";
        }

        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open()
        {
            var win = EditorWindow.GetWindow<ILRuntimeEditorWin>();
            win.titleContent = new GUIContent("ILRuntime Manager");
            win.minSize = new Vector2(1000, 300);
            win.maxSize = new Vector2(600, 300);
            win.Show();
        }

        ConfigVO cfg;

        private void OnEnable()
        {
            cfg = LoadConfig<ConfigVO>(CONFIG_NAME);
            if (null == cfg)
            {
                cfg = new ConfigVO();
            }
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

            GUILayout.BeginHorizontal();
            cfg.bindingCodeDir = EditorGUILayout.TextField("绑定代码发布目录:", cfg.bindingCodeDir);
            if (GUILayout.Button("选择目录", GUILayout.Width(100)))
            {
                var dir = UnityEditor.EditorUtility.OpenFolderPanel("选择发布目录", "", "");
                if (false == string.IsNullOrEmpty(dir))
                {
                    dir = string.Format("Assets{0}", dir.Replace(Application.dataPath, ""));
                    cfg.bindingCodeDir = dir;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("生成绑定代码", GUILayout.Width(100)))
            {
                var dllFile = UnityEditor.EditorUtility.OpenFilePanel("选择热更DLL", Application.dataPath, "dll");
                if (false == string.IsNullOrEmpty(dllFile))
                {
                    GenerateCLRBindingByAnalysis(dllFile, cfg.bindingCodeDir);
                }
            }

            if (GUILayout.Button("清空绑定代码", GUILayout.Width(100)))
            {
                if(FileUtil.DeleteFileOrDirectory(cfg.bindingCodeDir))
                {
                    EditorUtility.DisplayDialog("提示", "成功！", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "失败！", "OK");
                }
            }

            GUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        void GenerateCLRBindingByAnalysis(string dllFile, string generatedDir)
        {
            //用新的分析热更dll调用引用来生成绑定代码
            ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain();            

            using (System.IO.FileStream fs = new System.IO.FileStream(dllFile, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                domain.LoadAssembly(fs);

                #region 这里需要注册所有热更DLL中用到的跨域继承Adapter，否则无法正确抓取引用        
                domain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
                #endregion

                ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain, generatedDir);
            }
        }
    }
}