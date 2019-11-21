using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using UnityEditor;
using UnityEngine;
using ZeroEditor;

namespace ZeroEditor
{
    class AZeroMenuEditorWindow<T> : OdinMenuEditorWindow where T : EditorWindow
    {
        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open(string title)
        {
            var win = GetWindow<T>(title, true);
            var rect = GUIHelper.GetEditorWindowRect().AlignCenter(1000, 600);
            win.position = rect;
        }

        OdinMenuTree _tree;
        public OdinMenuTree menuTree
        {
            get
            {
                if(null == _tree)
                {
                    _tree = new OdinMenuTree();                    
                    _tree.Config.DrawSearchToolbar = true;                    
                }
                return _tree;
            }
        }

        protected override OdinMenuTree BuildMenuTree()
        {            
            //子类重写
            return _tree;
        }

        protected void AddModule<Module>(string path, EditorIcon icon = null) where Module : AEditorModule
        {            
            if (null == icon)
            {
                menuTree.Add(path, Activator.CreateInstance(typeof(Module), new object[] { this }));
            }
            else
            {
                menuTree.Add(path, Activator.CreateInstance(typeof(Module), new object[] { this }), icon);
            }
        }        
    }
}
