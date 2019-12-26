using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;

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

        List<AEditorModule> _modules = new List<AEditorModule>();

        OdinMenuTree _tree;
        AEditorModule _currentModule;
        public OdinMenuTree menuTree
        {
            get
            {
                if(null == _tree)
                {
                    _tree = new OdinMenuTree();                    
                    _tree.Config.DrawSearchToolbar = true;
                    _tree.Selection.SelectionChanged += SelectionChanged;
                }
                return _tree;
            }
        }

        private void SelectionChanged(SelectionChangedType sct)
        {
            switch (sct)
            {
                case SelectionChangedType.ItemRemoved:
                    break;
                case SelectionChangedType.ItemAdded:
                    if (null != _tree.Selection.SelectedValue)
                    {
                        _currentModule = _tree.Selection.SelectedValue as AEditorModule;
                        _currentModule.OnEnable();
                    }
                    break;
                case SelectionChangedType.SelectionCleared:
                    if (null != _currentModule)
                    {
                        _currentModule.OnDisable();
                        _currentModule = null;
                    }
                    break;
            }            
        }

        protected override OdinMenuTree BuildMenuTree()
        {            
            //子类重写
            return _tree;
        }

        /// <summary>
        /// 选中指定模块匹配的菜单项
        /// </summary>
        /// <typeparam name="Module"></typeparam>
        public void Select<Module>()
        {
            foreach(var module in _modules)
            {
                if(module.GetType() == typeof(Module))
                {
                    TrySelectMenuItemWithObject(module);
                    break;
                }
            }
        }

        protected void AddModule<Module>(string path, EditorIcon icon = null) where Module : AEditorModule
        {
            var module = Activator.CreateInstance(typeof(Module), new object[] { this }) as AEditorModule;
            if (null == icon)
            {
                menuTree.Add(path, module);
            }
            else
            {
                menuTree.Add(path, module, icon);
            }
            _modules.Add(module);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach(var module in _modules)
            {
                module.OnDisable();
            }
            _modules.Clear();            
        }
    }
}
