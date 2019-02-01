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

        /// <summary>
        /// 命名空间节点
        /// </summary>
        public class NamespaceNodeVO
        {
            public string name;
            public List<string> typeNameList = new List<string>();

            public NamespaceNodeVO(string name)
            {
                this.name = name;
            }
        }

        /// <summary>
        /// 动态库节点
        /// </summary>
        public class AssemblyNodeVO
        {
            public string name;
            public List<NamespaceNodeVO> nsNodeList = new List<NamespaceNodeVO>();

            public AssemblyNodeVO(string name)
            {
                this.name = name;
            }
        }

        List<AssemblyNodeVO> _nodeList;

        public List<AssemblyNodeVO> NodeList
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
            _nodeList = new List<AssemblyNodeVO>();
            var files = Directory.GetFiles(_dir, "*.dll", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var vo = CreateNodeVO(file);
                if (null != vo)
                {
                    _nodeList.Add(vo);
                }
            }

            _linkXMLString = CreateXML(_nodeList);

            if (null != onCreated)
            {
                onCreated.Invoke(this);
            }
        }

        string CreateXML(List<AssemblyNodeVO> assemblyNodeList)
        {
            //创建xml实例；
            XmlDocument xmlDoc = new XmlDocument();

            //创建xml根节点，最上层节点
            XmlElement root = xmlDoc.CreateElement("linker");
            xmlDoc.AppendChild(root);

            foreach (var assemblyNode in assemblyNodeList)
            {
                //创建Assembly节点
                XmlElement assembly = xmlDoc.CreateElement("assembly");
                assembly.SetAttribute("fullname", assemblyNode.name);

                foreach (var nsNode in assemblyNode.nsNodeList)
                {
                    XmlElement ns = xmlDoc.CreateElement("namespace");
                    ns.SetAttribute("fullname", nsNode.name);
                    ns.SetAttribute("preserve", "all");
                    assembly.AppendChild(ns);
                }

                root.AppendChild(assembly);
            }

            return xmlDoc.InnerXml;
        }

        AssemblyNodeVO CreateNodeVO(string dllFile)
        {
            var dllBytes = File.ReadAllBytes(dllFile);
            var assembly = Assembly.Load(dllBytes);
            
            Type[] typeList = null;
            try
            {
                typeList = assembly.GetTypes();
            }
            catch(Exception e)
            {
                Debug.LogErrorFormat("无法处理的Assembly:{0}", dllFile);
            }

            if(null == typeList)
            {
                return null;
            }

            AssemblyNodeVO vo = new AssemblyNodeVO(Path.GetFileNameWithoutExtension(dllFile));
            Dictionary<string, NamespaceNodeVO> _nsDic = new Dictionary<string, NamespaceNodeVO>();

            foreach (var type in typeList)
            {
                string nsName = null;
                string typeName = null;
                try
                {
                    nsName = type.Namespace;
                    typeName = type.Name;
                }
                catch
                {
                    Debug.LogErrorFormat("无法处理的Type:{0}", type);
                    nsName = null;
                    typeName = null;
                }

                if(null == nsName || null == typeName)
                {
                    continue;
                }

                if (false == _nsDic.ContainsKey(type.Namespace))
                {
                    _nsDic.Add(type.Namespace, new NamespaceNodeVO(type.Namespace));
                }

                _nsDic[type.Namespace].typeNameList.Add(type.Name);
            }

            foreach (var entry in _nsDic)
            {
                vo.nsNodeList.Add(entry.Value);
            }

            return vo;
        }
    }
}
