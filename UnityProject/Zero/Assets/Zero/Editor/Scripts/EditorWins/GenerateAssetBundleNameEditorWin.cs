using Jing;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    public class GenerateAssetBundleNameEditorWin : OdinEditorWindow
    {        
        public const string TEMPLATE_FILE = "Assets/Zero/Editor/Configs/AssetBundleNameClassTemplate.txt";

        public const string OUTPUT_CLASS_FILE = "Assets/@Scripts/Generated/AssetBundleName.cs";

        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open()
        {
            var win = GetWindow<GenerateAssetBundleNameEditorWin>("自动生成AssetBundleName.cs", true);
            win.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);

            
        }

        
        public struct AssetBundleItemVO
        {
            /// <summary>
            /// 注释
            /// </summary>
            public string explain;

            /// <summary>
            /// assetbundle名称
            /// </summary>
            public string assetbundle;
        }

        [LabelText("开始生成"), Button(size:ButtonSizes.Large)]
        void GeneratedAssetBundleNameClass()
        {
            var template = File.ReadAllText(TEMPLATE_FILE);
            StringBuilder sb = new StringBuilder();
            string fieldTemplate = "\t\tpublic const string {0} = \"{1}.ab\";";
            sb.AppendLine(string.Format(fieldTemplate, "A", "a"));
            sb.AppendLine();
            sb.AppendLine(string.Format(fieldTemplate, "B", "b"));
            sb.AppendLine();
            sb.AppendLine(string.Format(fieldTemplate, "C", "c"));
            var classContent = template.Replace("{0}", sb.ToString());
            File.WriteAllText(OUTPUT_CLASS_FILE, classContent);


            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(OUTPUT_CLASS_FILE, 0);
        }
    }



}
