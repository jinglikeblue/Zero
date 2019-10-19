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
    public class ILRuntimeEditorWin : OdinEditorWindow
    {
        const string CONFIG_NAME = "ilruntime_config.json";

        [HideLabel]
        [Serializable]
        public class ConfigVO
        {
            [LabelText("绑定代码发布目录"), FolderPath]
            public string bindingCodeDir;
        }

        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open()
        {
            var win = GetWindow<ILRuntimeEditorWin>("ILRuntime", true);
            win.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        }        

        override protected void OnEnable()
        {
            base.OnEnable();
            cfg = EditorConfigUtil.LoadConfig<ConfigVO>(CONFIG_NAME);
        }

        [LabelText("保存配置"), Button(size:ButtonSizes.Medium), PropertyOrder(-1)]
        void SaveConfig()
        {
            EditorConfigUtil.SaveConfig(cfg, CONFIG_NAME);
            ShowNotification(new GUIContent("保存成功"));
        }

        public ConfigVO cfg;

        [HorizontalGroup("BottomButtons")]
        [LabelText("生成绑定代码"), Button]
        void GenerateCLRBindingScripts()
        {
            var dllFile = UnityEditor.EditorUtility.OpenFilePanel("选择热更DLL", Application.dataPath, "dll");
            if (false == string.IsNullOrEmpty(dllFile))
            {
                GenerateCLRBindingByAnalysis(dllFile, cfg.bindingCodeDir);
            }

            EditorUtility.DisplayDialog("提示", "成功！", "OK");
            AssetDatabase.Refresh();
        }

        [HorizontalGroup("BottomButtons")]
        [LabelText("清空绑定代码"), Button]
        void ClearCLRBindingScripts()
        {
            if (FileUtil.DeleteFileOrDirectory(cfg.bindingCodeDir))
            {
                EditorUtility.DisplayDialog("提示", "成功！", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("提示", "失败！", "OK");
            }

            AssetDatabase.Refresh();
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