using Jing;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    public class GenerateAssetBundleNameEditorWin : OdinEditorWindow
    {        
        public const string TEMPLATE_FILE = "Assets/Zero/Editor/Configs/AssetBundleNameClassTemplate.txt";

        public const string OUTPUT_CLASS_FILE = "Assets/@Scripts/Generated/AssetBundleName1.cs";

        public const string FIELD_EXPLAIN_FORMAT = "\t\t" + @"/// <summary>
        /// {0}
        /// </summary>";

        public const string FIELD_FORMAT = "\t\tpublic const string {0} = \"{1}.ab\";";


        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void Open()
        {
            var win = GetWindow<GenerateAssetBundleNameEditorWin>("自动生成AssetBundleName.cs", true);
            win.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);

            
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            abList = FindAssetBundles(ZeroConst.HOT_RESOURCES_ROOT_DIR);
        }
        
        [ListDrawerSettings(IsReadOnly =true)]
        public List<AssetBundleItemVO> abList;
        
        public struct AssetBundleItemVO
        {
            /// <summary>
            /// 注释
            /// </summary>
            [LabelText("添加注释")]
            public string explain;

            /// <summary>
            /// assetbundle名称
            /// </summary>
            [LabelText("$GetFieldName")]
            [DisplayAsString]
            public string assetbundle;

            public string GetFieldName()
            {
                return assetbundle.Replace("/", "_").Replace(".ab","").ToUpper();
            }
        }

        [LabelText("开始生成"), Button(size:ButtonSizes.Large)]
        void GeneratedAssetBundleNameClass()
        {
            var dir = Directory.GetParent(OUTPUT_CLASS_FILE);
            if (false == dir.Exists)
            {
                dir.Create();
            }

            var template = File.ReadAllText(TEMPLATE_FILE);
            StringBuilder sb = new StringBuilder();

            foreach(var vo in abList)
            {
                if(false == string.IsNullOrEmpty(vo.explain))
                {
                    sb.AppendLine(string.Format(FIELD_EXPLAIN_FORMAT, vo.explain));
                }                
                sb.AppendLine(string.Format(FIELD_FORMAT, vo.GetFieldName(), vo.assetbundle));
                sb.AppendLine();
            }

            var classContent = template.Replace("{0}", sb.ToString());
            File.WriteAllText(OUTPUT_CLASS_FILE, classContent);


            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(OUTPUT_CLASS_FILE, 0);
        }

        /// <summary>
        /// 找出所有要打包的资源
        /// </summary>
        List<AssetBundleItemVO> FindAssetBundles(string resRootDir)
        {
            List<AssetBundleItemVO> list = new List<AssetBundleItemVO>();
            string[] dirs = Directory.GetDirectories(resRootDir, "*", SearchOption.AllDirectories);
            foreach(var dir in dirs)
            {                
                var di = new DirectoryInfo(dir);
                if(di.GetFiles().Length == 0)
                {
                    continue;
                }
                
                AssetImporter ai = AssetImporter.GetAtPath(dir);
                string abName = ai.assetPath.Substring(resRootDir.Length + 1) + ZeroConst.AB_EXTENSION;

                AssetBundleItemVO vo;
                vo.explain = "";
                vo.assetbundle = abName;
                list.Add(vo);
            }
            return list;
        }
    }



}
