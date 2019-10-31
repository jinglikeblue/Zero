using Jing;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ZeroEditor
{
    public class GenerateLinkXMLModule : AEditorModule
    {
        public GenerateLinkXMLModule(EditorWindow editorWin) : base(editorWin)
        {
        }

        [Space(10)]
        [LabelText("Includes"), ListDrawerSettings(ShowPaging = false, IsReadOnly = true)]
        public List<CreateLinkXMLCommand.AssemblyNodeVO> nodeList;
        CreateLinkXMLCommand _cmd;

        [Title("link.xml 生成", TitleAlignment = TitleAlignments.Centered)]
        [LabelText("选择DLL所在目录"), Button(size: ButtonSizes.Large), PropertyOrder(-1)]
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

        [PropertySpace(10)]
        [LabelText("导出 [link.xml]"), Button(size: ButtonSizes.Large)]
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
            editorWin.ShowTip("导出完毕!");
        }
    }
}
