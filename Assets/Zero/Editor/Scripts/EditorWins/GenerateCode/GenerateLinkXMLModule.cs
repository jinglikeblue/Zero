using Jing;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace ZeroEditor
{
    /// <summary>
    /// 生成「link.xml」文件模块
    /// </summary>
    public class GenerateLinkXMLModule : AEditorModule
    {
        string CONFIG_NAME = "generate_link_xml_config.json";    
        
        const string NONEXISTENT_FLAG = "[*不存在*]";

        struct ConfigVO
        {
            /// <summary>
            /// 要引入的文件夹
            /// </summary>
            public List<string> includeDirs;

            /// <summary>
            /// 要引入的Dll列表
            /// </summary>
            public List<string> includeDlls;
        }

        ConfigVO _cfg;

        public GenerateLinkXMLModule(EditorWindow editorWin) : base(editorWin)
        {
            _cfg = EditorConfigUtil.LoadConfig<ConfigVO>(CONFIG_NAME);
            if (null == _cfg.includeDirs)
            {
                _cfg.includeDirs = new List<string>();
            }
            if (null == _cfg.includeDlls)
            {
                _cfg.includeDlls = new List<string>();
            }

            includeDirs = _cfg.includeDirs;
            includeDlls = _cfg.includeDlls;

            CheckExistsAndRefreshPreviewList();            
        }

        void CheckExistsAndRefreshPreviewList()
        {
            for (int i = 0; i < includeDirs.Count; i++)
            {
                if (false == Directory.Exists(includeDirs[i]))
                {
                    includeDirs[i] = $"{NONEXISTENT_FLAG}{includeDirs[i]}";
                }
            }

            for (int i = 0; i < includeDlls.Count; i++)
            {
                if (false == File.Exists(includeDlls[i]))
                {
                    includeDlls[i] = $"{NONEXISTENT_FLAG}{includeDlls[i]}";
                }
            }

            var defaultDir = FileUtility.CombineDirs(true, EditorApplication.applicationContentsPath);
            if (false == includeDirs.Contains(defaultDir))
            {
                includeDirs.Add(defaultDir);
                Debug.LogFormat("[{0}]是必须的", defaultDir);                
            }

            RefreshPreviewList();
        }

        void SaveConfig()
        {
            for (int i = 0; i < includeDirs.Count; i++)
            {
                includeDirs[i] = includeDirs[i].Replace(NONEXISTENT_FLAG, "");
            }

            for (int i = 0; i < includeDlls.Count; i++)
            {
                includeDlls[i] = includeDlls[i].Replace(NONEXISTENT_FLAG, "");
            }

            EditorConfigUtil.SaveConfig(_cfg, CONFIG_NAME);

            CheckExistsAndRefreshPreviewList();
        }

        [Title("link.xml 生成", TitleAlignment = TitleAlignments.Centered)]
        [InfoBox("IL2CPP在打包时会自动对Unity工程的DLL进行裁剪，将代码中没有引用到的类型裁剪掉，以达到减小发布后ipa包的尺寸的目的。然而在实际使用过程中，很多类型有可能会被意外剪裁掉，造成运行时抛出找不到某个类型的异常。特别是通过反射等方式在编译时无法得知的函数调用，在运行时都很有可能遇到问题。" +
            "Unity提供了一个方式来告诉Unity引擎，哪些类型是不能够被剪裁掉的。具体做法就是在Unity工程的Assets目录中建立一个叫link.xml的XML文件指定你需要保留的类型")]
        [LabelText("导出位置"), DisplayAsString, PropertyOrder(-999)]
        public string path = "assets/link.xml";

        [HorizontalGroup("AddButtons", order: -1)]
        [LabelText("添加扫描目录"), Button(size: ButtonSizes.Large)]
        void AddDir()
        {
            string dir = EditorUtility.OpenFolderPanel("扫描目录", Application.dataPath, "");
            if (string.IsNullOrEmpty(dir) || includeDirs.Contains(dir))
            {
                return;
            }
            
            var relativePath = FileUtility.GetRelativePath(ZeroEditorConst.PROJECT_PATH, dir);
            if(null != relativePath)
            {
                dir = relativePath;
            }

            includeDirs.Add(dir);
            OnListChange();
        }

        [HorizontalGroup("AddButtons")]
        [LabelText("添加Dll文件"), Button(size: ButtonSizes.Large)]
        void AddDll()
        {
            string dll = EditorUtility.OpenFilePanel("Dll文件", Application.dataPath, "dll");
            if (string.IsNullOrEmpty(dll) || includeDlls.Contains(dll))
            {
                return;
            }

            var fi = new FileInfo(dll);

            var relativePath = FileUtility.GetRelativePath(ZeroEditorConst.PROJECT_PATH, fi.DirectoryName);
            if (null != relativePath)
            {
                dll = FileUtility.CombinePaths(relativePath, fi.Name);
            }            

            includeDlls.Add(dll);
            OnListChange();
        }

        [PropertySpace(10)]
        [LabelText("扫描的Dll文件夹列表"), ListDrawerSettings(Expanded = true, HideAddButton = true, DraggableItems = false)]
        [DisplayAsString]
        [OnValueChanged("OnListChange", includeChildren: true)]
        public List<string> includeDirs = new List<string>();

        [PropertySpace(10)]
        [LabelText("添加的Dll文件列表"), ListDrawerSettings(Expanded = true, HideAddButton = true, DraggableItems = false)]
        [DisplayAsString]
        [OnValueChanged("OnListChange", includeChildren: true)]
        public List<string> includeDlls = new List<string>();

        void OnListChange()
        {
            Debug.Log("列表数据变化");            
            SaveConfig();
        }

        [PropertySpace(10)]
        [LabelText("导出 [link.xml]"), Button(size: ButtonSizes.Large)]
        void CreateLinkXML()
        {
            const string OUTPUT_FILE = "Assets/link.xml";

            new GenerateLinkXMLCommand(outputPreviewList, OUTPUT_FILE).Excute();

            AssetDatabase.Refresh();
            ShowTip("[{0}] 导出完毕!", OUTPUT_FILE);
        }

        [PropertySpace(10)]
        [LabelText("保留类型预览"), DisplayAsString, PropertyOrder(999), ListDrawerSettings(DraggableItems = false, HideRemoveButton = true, HideAddButton = true, NumberOfItemsPerPage = 20)]
        public List<string> outputPreviewList;

        /// <summary>
        /// 刷新导出预览列表
        /// </summary>
        void RefreshPreviewList()
        {
            HashSet<string> assemblySet = new HashSet<string>();

            foreach (var dll in includeDlls)
            {
                if (File.Exists(dll))
                {
                    assemblySet.Add(Path.GetFileNameWithoutExtension(dll));
                }
            }

            foreach (var dir in includeDirs)
            {
                if (Directory.Exists(dir))
                {
                    var files = Directory.GetFiles(dir, "*.dll", SearchOption.AllDirectories);
                    for (int i = 0; i < files.Length; i++)
                    {
                        var file = files[i];
                        assemblySet.Add(Path.GetFileNameWithoutExtension(file));
                    }
                }
            }

            if (null == outputPreviewList)
            {
                outputPreviewList = new List<string>();
            }
            else
            {
                outputPreviewList.Clear();
            }
            outputPreviewList.AddRange(assemblySet);
        }
    }
}
