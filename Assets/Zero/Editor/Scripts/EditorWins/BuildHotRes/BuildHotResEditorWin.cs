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
    class BuildHotResEditorWin : OdinMenuEditorWindow
    {
        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open()
        {
            var win = GetWindow<BuildHotResEditorWin>("热更新", true);
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
            tree.Add("热更资源", new BuildResModule(this));
            tree.Add("setting.json", new BuildSettingJsonModule(this));            
            return tree;
        }
    }
}
