using Jing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEditor;
using UnityEngine;
using Zero.Edit;

namespace Zero.Edit
{
    class LinkerXMLEditorWin : AEditorWin
    {
        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open()
        {
            var win = EditorWindow.GetWindow<LinkerXMLEditorWin>("Useless Assets Find", true);
            win.minSize = new Vector2(800, 700);
            win.maxSize = new Vector2(1000, 700);
            win.Show();
        }

        string _selectDir;

        private void OnGUI()
        {
            if (GUILayout.Button("选择DLL所在目录", GUILayout.Height(30)))
            {
                string dir = EditorUtility.OpenFolderPanel("Dll文件目录", Application.dataPath, "");
                _selectDir = dir;
                var cmd = new CreateLinkXMLCommand(dir);
                cmd.onCreated += OnCreated;
                cmd.Excute();
            }
        }

        private void OnCreated(CreateLinkXMLCommand cmd)
        {
            cmd.onCreated -= OnCreated;
            var savePath = FileSystem.CombinePaths(_selectDir, "linker.xml");
            File.WriteAllText(savePath, cmd.LinkXMLString);
            //打开目录
            EditorMenu.OpenDirectory(_selectDir);
            Debug.Log("创建成功");
        }
    }

    class CreateLinkXMLCommand
    {
        string _dir;

        public class NodeVO
        {
            public string assemblyName;
            public List<string> typeNameList = new List<string>();
            public List<string> namespaceNameList = new List<string>();
        }

        List<NodeVO> _nodeList;

        public List<NodeVO> NodeList
        {
            get { return _nodeList; }
        }

        string _linkXMLString;

        public string LinkXMLString
        {
            get
            {
                return _linkXMLString;
            }
        }

        public event Action<CreateLinkXMLCommand> onCreated;

        public CreateLinkXMLCommand(string dir)
        {            
            _dir = dir;
        }

        public void Excute()
        {
            _nodeList = new List<NodeVO>();
            var files = Directory.GetFiles(_dir, "*.dll", SearchOption.AllDirectories);
            for(int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var vo = CreateNodeVO(file);
                if (null != vo)
                {
                    _nodeList.Add(vo);
                }
            }

            _linkXMLString = CreateXML(_nodeList);

            if(null != onCreated)
            {
                onCreated.Invoke(this);
            }

            
        }

        string CreateXML(List<NodeVO> nodeList)
        {
            //创建xml实例；
            XmlDocument xmlDoc = new XmlDocument();

            //创建xml根节点，最上层节点
            XmlElement root = xmlDoc.CreateElement("linker");
            xmlDoc.AppendChild(root);

            foreach(var node in nodeList)
            {
                //创建Assembly节点
                XmlElement assembly = xmlDoc.CreateElement("assembly");
                assembly.SetAttribute("fullname", node.assemblyName);

                foreach(var nsName in node.namespaceNameList)
                {
                    XmlElement ns = xmlDoc.CreateElement("namespace");
                    ns.SetAttribute("fullname", nsName);
                    ns.SetAttribute("preserve", "all");
                    assembly.AppendChild(ns);
                }

                root.AppendChild(assembly);
            }
            
            return xmlDoc.InnerXml;
        }

        NodeVO CreateNodeVO(string dllFile)
        {
            NodeVO vo = new NodeVO();

            vo.assemblyName = Path.GetFileNameWithoutExtension(dllFile);

            var assembly = Assembly.LoadFile(dllFile);            
            try
            {
                var typeList = assembly.GetTypes();
                foreach (var type in typeList)
                {
                    vo.typeNameList.Add(type.Name);

                    if (false == vo.namespaceNameList.Contains(type.Namespace))
                    {
                        vo.namespaceNameList.Add(type.Namespace);
                    }
                }
            }
            catch
            {
                Debug.LogErrorFormat("无法处理的DLL：{0}", dllFile);
                vo = null;
            }
            return vo;
        }
    }
}
