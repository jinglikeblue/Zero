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

namespace ZeroEditor
{
    public class GenerateCodeEditorWin : OdinMenuEditorWindow
    {
        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open()
        {            
            var win = GetWindow<GenerateCodeEditorWin>("代码生成", true);
            var rect = GUIHelper.GetEditorWindowRect().AlignCenter(1000, 600);
            win.position = rect;
        }        

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override OdinMenuTree BuildMenuTree()
        {            
            OdinMenuTree tree = new OdinMenuTree();
            tree.Config.DrawSearchToolbar = true;
            tree.Add("Assets资源名生成", new GenerateAssetBundleNameModule(this));
            tree.Add("link.xml", new GenerateLinkXMLModule(this));                       
            tree.Add("ILRuntime CLR Binding Codes", new GenerateILRuntimeCLRBindingModule(this));
            return tree;
        }
    }
}
