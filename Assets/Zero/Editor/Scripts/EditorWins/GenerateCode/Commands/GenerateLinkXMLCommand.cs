using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace ZeroEditor
{
    class GenerateLinkXMLCommand
    {
        List<string> _list;
        string _outputPath;
        public GenerateLinkXMLCommand(List<string> list, string outputPath)
        {
            _list = list;
            _outputPath = outputPath;
        }

        public void Excute()
        {
            File.WriteAllText(_outputPath, CreateXML(), Encoding.UTF8);
        }

        string CreateXML()
        {
            //创建xml实例；
            XmlDocument xmlDoc = new XmlDocument();

            //创建xml根节点，最上层节点
            XmlElement root = xmlDoc.CreateElement("linker");
            xmlDoc.AppendChild(root);

            foreach (var assemblyName in _list)
            {
                //创建Assembly节点
                XmlElement assembly = xmlDoc.CreateElement("assembly");
                assembly.SetAttribute("fullname", assemblyName);
                assembly.SetAttribute("preserve", "all");
                root.AppendChild(assembly);
            }

            return xmlDoc.InnerXml;
        }
    }
}
