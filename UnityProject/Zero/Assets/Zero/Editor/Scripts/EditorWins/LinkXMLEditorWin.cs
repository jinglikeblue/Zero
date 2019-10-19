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

namespace Zero.Edit
{
    class LinkXMLEditorWin : OdinEditorWindow
    {
        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open()
        {
            var win = GetWindow<LinkXMLEditorWin>("link.xml 工具", true);
            win.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        }              
        
        [LabelText("Includes"), ListDrawerSettings(ShowPaging = false, IsReadOnly = true)]
        public List<CreateLinkXMLCommand.AssemblyNodeVO> nodeList;
        CreateLinkXMLCommand _cmd;

        [LabelText("选择DLL所在目录"),Button(size:ButtonSizes.Medium), PropertyOrder(-1)]
        void SelectDllDir()
        {
            string dir = EditorUtility.OpenFolderPanel("Dll文件目录", Application.dataPath, "");
            if (string.IsNullOrEmpty(dir))
            {
                return;
            }            
            _cmd = new CreateLinkXMLCommand(dir);
            _cmd.onCreated += OnCreated;
            _cmd.Excute();
        }

        private void OnCreated(CreateLinkXMLCommand cmd)
        {
            nodeList = _cmd.NodeList;
            _cmd.onCreated -= OnCreated;            
        }

        [LabelText("导出 [link.xml]"), Button(size: ButtonSizes.Medium)]
        void CreateLinkXML()
        {
            string saveDir = EditorUtility.OpenFolderPanel("选择保存位置", Application.dataPath, "");
            if (string.IsNullOrEmpty(saveDir))
            {
                return;
            }
            var savePath = FileSystem.CombinePaths(saveDir, "link.xml");
            File.WriteAllText(savePath, _cmd.LinkXMLString);
            //打开目录
            ZeroEditorUtil.OpenDirectory(saveDir);
            ShowNotification(new GUIContent("导出完毕!"));            
        }     
    }
}
