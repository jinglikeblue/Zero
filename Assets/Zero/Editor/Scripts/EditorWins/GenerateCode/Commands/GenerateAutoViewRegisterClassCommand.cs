using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZeroEditor
{
    class GenerateAutoViewRegisterClassCommand : BaseGenerateTemplateCodeCommand
    {
        /// <summary>
        /// 模板文件位置
        /// </summary>
        public const string TEMPLATE_FILE = "Assets/Zero/Editor/Configs/ViewAutoRegisterTemplate.txt";

        /// <summary>
        /// 导出类位置
        /// </summary>
        public const string OUTPUT_FILE = "Assets/@Scripts/Generated/ViewAutoRegister.cs";
        //public const string OUTPUT_FILE = "ViewAutoRegister.cs";

        public readonly List<AssetBundleItemVO> abList;

        string _classT;
        string _registerT;

        string _ns;

        public GenerateAutoViewRegisterClassCommand(List<AssetBundleItemVO> abList, string ns)
        {
            _ns = ns;
            this.abList = abList;
        }

        public override void Excute()
        {
            var dir = Directory.GetParent(OUTPUT_FILE);
            if (false == dir.Exists)
            {
                dir.Create();
            }

            var template = File.ReadAllText(TEMPLATE_FILE).Split(new string[] { TEMPLATE_SPLIT }, StringSplitOptions.RemoveEmptyEntries);
            _classT = template[0];
            _registerT = template[1].Replace("\r\n", "");

            string classContent = _classT.Replace(FIELD_LIST_FLAG, GenerateRegisterItems());
            classContent = classContent.Replace(NAMESPACE_FLAG, _ns);

            File.WriteAllText(OUTPUT_FILE, classContent);
        }

        string GenerateRegisterItems()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var ab in abList)
            {
                foreach (var viewName in ab.assetList)
                {
                    if (Path.GetExtension(viewName).Equals(".prefab"))
                    {
                        var view = Path.GetFileNameWithoutExtension(viewName);
                        sb.Append(_registerT.Replace(FIELD_NAME_FLAG, ab.assetbundle).Replace(FIELD_VALUE_FLAG, view));
                        sb.AppendLine();
                    }
                }
            }

            return sb.ToString();
        }
    }
}
