using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEngine;

namespace Zero.Edit
{
    public class CreateLinkXMLCommand
    {
        public event Action<CreateLinkXMLCommand> onCreated;

        string _dir;

        string _linkXMLString;

        List<AssemblyNodeVO> _nodeList;

        /// <summary>
        /// 命名空间节点
        /// </summary>
        [HideLabel]
        [Serializable]
        public class NamespaceNodeVO
        {
            [HideInEditorMode]
            public string name;
            [LabelText("$Label"), ListDrawerSettings(ShowPaging = false, IsReadOnly = true)]
            [DisplayAsString]
            public List<string> typeNameList = new List<string>();

            public NamespaceNodeVO(string name)
            {
                this.name = name;
            }

            string Label()
            {
                return string.Format("{0} [Namespace]", name);
            }
        }

        /// <summary>
        /// 动态库节点
        /// </summary>
        [HideLabel]
        [Serializable]
        public class AssemblyNodeVO
        {
            [HideInEditorMode]
            public string name;
            [LabelText("$Label"), ListDrawerSettings(ShowPaging = false, IsReadOnly = true)]
            public List<NamespaceNodeVO> nsNodeList = new List<NamespaceNodeVO>();

            public AssemblyNodeVO(string name)
            {
                this.name = name;
            }

            string Label()
            {
                return string.Format("{0}.dll", name);
            }
        }        

        public List<AssemblyNodeVO> NodeList
        {
            get { return _nodeList; }
        }        

        public string LinkXMLString
        {
            get
            {
                return _linkXMLString;
            }
        }       

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
                assembly.SetAttribute("preserve", "all");

                //foreach (var nsNode in assemblyNode.nsNodeList)
                //{
                //    XmlElement ns = xmlDoc.CreateElement("namespace");
                //    ns.SetAttribute("fullname", nsNode.name);
                //    ns.SetAttribute("preserve", "all");
                //    assembly.AppendChild(ns);
                //}

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
            catch (Exception e)
            {
                Debug.LogErrorFormat("无法处理的Assembly:{0}", dllFile);
            }

            if (null == typeList)
            {
                return null;
            }

            if (0 == typeList.Length)
            {
                Debug.LogWarningFormat("Types长度为0: {0}", dllFile);                
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

                if (null == nsName || null == typeName)
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
