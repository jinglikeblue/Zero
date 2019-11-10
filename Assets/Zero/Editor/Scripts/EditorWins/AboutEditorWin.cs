using Jing;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ZeroEditor
{
    public class AboutEditorWin: OdinEditorWindow
    {
        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open()
        {
            var win = GetWindow<AboutEditorWin>("关于", true);
            win.position = GUIHelper.GetEditorWindowRect().AlignCenter(320, 140);
        }

        [PropertySpace(20)]
        [LabelText("作者"), DisplayAsString]
        public string vuthor = "Jing";

        [LabelText("版本"),DisplayAsString]
        public string version = "2.0";

        [PropertySpace(10)]
        [HorizontalGroup("Buttons")]
        [LabelText("文档"), Button]
        void OpenDocument()
        {
            //访问网站
            Application.OpenURL(@"https://jinglikeblue.github.io/Zero/Docs/Intro");
        }

        [PropertySpace(10)]
        [HorizontalGroup("Buttons")]
        [LabelText("GitHub"), Button]
        void OpenGitHub()
        {
            //访问网站
            Application.OpenURL(@"https://github.com/jinglikeblue/Zero");
        }
    }
}
