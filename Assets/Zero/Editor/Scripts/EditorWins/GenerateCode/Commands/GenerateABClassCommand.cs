using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZeroEditor
{
    class GenerateABClassCommand : BaseGenerateTemplateCodeCommand
    {
        /// <summary>
        /// AB名称类模板文件位置
        /// </summary>
        public const string TEMPLATE_FILE = "Assets/Zero/Editor/Configs/AssetBundleNameClassTemplate.txt";

        /// <summary>
        /// 导出类位置
        /// </summary>
        public const string OUTPUT_FILE = "Assets/@Scripts/Generated/AB.cs";

        string _mainClassT;
        string _explainT;
        string _classT;
        string _fieldT;

        public readonly List<AssetBundleItemVO> abList;

        public GenerateABClassCommand(List<AssetBundleItemVO> abList)
        {
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
            _mainClassT = template[0];
            _explainT = template[1];
            _classT = template[2];
            _fieldT = template[3];

            string classContent;
            var mainClassName = Path.GetFileNameWithoutExtension(OUTPUT_FILE);
            classContent = _mainClassT.Replace(CLASS_NAME_FLAG, mainClassName);
            classContent = classContent.Replace(CLASS_LIST_FLAG, GenerateClassList());

            File.WriteAllText(OUTPUT_FILE, classContent);
        }

        string GenerateClassList()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var vo in abList)
            {
                var classContent = GenerateClass(vo);
                sb.Append(classContent);
                //sb.AppendLine();
            }

            return sb.ToString();
        }

        string GenerateClass(AssetBundleItemVO vo)
        {
            var fieldList = GenerateFieldList(vo.assetbundle, vo.assetList);

            var content = _classT;
            content = content.Replace(CLASS_NAME_FLAG, vo.GetFieldName());
            content = content.Replace(FIELD_LIST_FLAG, fieldList);
            if (string.IsNullOrEmpty(vo.explain))
            {
                content = content.Replace(EXPLAIN_FLAG, "");
            }
            else
            {
                content = content.Replace(EXPLAIN_FLAG, _explainT.Replace(EXPLAIN_FLAG, vo.explain));
            }
            return content;
        }

        string GenerateFieldList(string abName, List<string> viewNameList)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(GenerateFiled("NAME", abName));
            sb.AppendLine();

            foreach (var viewName in viewNameList)
            {
                var fieldName = Path.GetFileNameWithoutExtension(viewName);
                sb.Append(GenerateFiled(fieldName, viewName));
                //sb.AppendLine();
            }

            return sb.ToString();
        }

        string GenerateFiled(string fieldName, string fieldValue)
        {
            fieldName = fieldName.Replace(' ', '_');
            return _fieldT.Replace(FIELD_NAME_FLAG, fieldName).Replace(FIELD_VALUE_FLAG, fieldValue);
        }
    }
}
