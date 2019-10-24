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
            var dllFile = UnityEditor.EditorUtility.OpenFilePanel("选择热更DLL", ZeroConst.PUBLISH_RES_ROOT_DIR, "dll");
            if (false == string.IsNullOrEmpty(dllFile))
            {
                GenerateCLRBindingByAnalysis(dllFile, GENERATED_OUTPUT_DIR);
                EditorUtility.DisplayDialog("提示", "成功！", "OK");
                AssetDatabase.Refresh();
            }
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
