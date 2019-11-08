using Sirenix.OdinInspector;
using System;
using UnityEditor;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    public class GenerateILRuntimeCLRBindingModule : AEditorModule
    {
        const string CONFIG_NAME = "ilruntime_config.json";

        const string GENERATED_OUTPUT_DIR = "Assets/Zero/Libs/ILRuntime/Generated";


        [Title("ILRuntime CLR Binding 代码生成", TitleAlignment = TitleAlignments.Centered)]
        [LabelText("绑定代码发布目录"), DisplayAsString]
        public string outputDir = GENERATED_OUTPUT_DIR;

        public GenerateILRuntimeCLRBindingModule(EditorWindow editorWin) : base(editorWin)
        {
            
        }

        [PropertySpace(10)]
        [HorizontalGroup("BottomButtons")]
        [LabelText("生成绑定代码"), Button(ButtonSizes.Large)]
        void GenerateCLRBindingScripts()
        {
            EditorUtility.DisplayProgressBar("生成绑定代码", "清空旧的绑定代码", 0f);
            FileUtil.DeleteFileOrDirectory(GENERATED_OUTPUT_DIR);
            EditorUtility.DisplayProgressBar("生成绑定代码", "构建新的scripts.dll", 0.5f);            
            var cmd = new DllBuildCommand(ZeroEditorConst.HOT_SCRIPT_ROOT_DIR, ZeroEditorConst.DLL_PUBLISH_DIR);
            cmd.onFinished += OnDllBuildFinished;
            cmd.Execute();
        }

        private void OnDllBuildFinished(DllBuildCommand cmd, bool isSuccess)
        {
            cmd.onFinished -= OnDllBuildFinished;            
            if (isSuccess)
            {
                EditorUtility.DisplayProgressBar("生成绑定代码", "解析生成绑定代码", 0.9f);
                try
                {
                    //构建成功后开始解析生成绑定代码
                    GenerateCLRBindingByAnalysis(cmd.assemblyPath, GENERATED_OUTPUT_DIR);
                }
                catch(Exception e)
                {
                    Debug.LogError(e);
                    isSuccess = false;
                }                
            }

            if (isSuccess)
            {
                editorWin.ShowTip("完成!");
            }
            else
            {                
                editorWin.ShowTip("生成绑定代码失败!");
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        [PropertySpace(10)]
        [HorizontalGroup("BottomButtons")]
        [LabelText("清空绑定代码"), Button(ButtonSizes.Large)]
        void ClearCLRBindingScripts()
        {
            if (FileUtil.DeleteFileOrDirectory(GENERATED_OUTPUT_DIR))
            {                
                editorWin.ShowTip("完成!");
            }
            else
            {
                editorWin.ShowTip("目标不存在或产生错误!");                
            }

            AssetDatabase.Refresh();
        }

        void GenerateCLRBindingByAnalysis(string dllFile, string generatedDir)
        {
            //用新的分析热更dll调用引用来生成绑定代码
            ILRuntime.Runtime.Enviorment.AppDomain appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();

            using (System.IO.FileStream fs = new System.IO.FileStream(dllFile, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                appdomain.LoadAssembly(fs);

                #region 这里需要注册所有热更DLL中用到的跨域继承Adapter，否则无法正确抓取引用        
                new ILRuntimeRegisters(appdomain).Register();
                #endregion

                ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(appdomain, generatedDir);
            }
        }
    }
}
