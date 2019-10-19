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
    public class GenerateAssetBundleNameModule
    {
        const string CONFIG_NAME = "asset_bundle_name_config.json";

        const string TEMPLATE_FILE = "Assets/Zero/Editor/Configs/AssetBundleNameClassTemplate.txt";

        const string OUTPUT_CLASS_FILE = "Assets/@Scripts/Generated/AssetBundleName.cs";

        const string FIELD_EXPLAIN_FORMAT = "\t\t" + @"/// <summary>
        /// {0}
        /// </summary>";

        const string FIELD_FORMAT = "\t\tpublic const string {0} = \"{1}\";";

        Dictionary<string, AssetBundleItemVO> _lastFindDic;

        public GenerateAssetBundleNameModule()
        {
            _lastFindDic = LoadConfig();

            new Thread(FindAssetBundles).Start();
        }

        Dictionary<string, AssetBundleItemVO> LoadConfig()
        {
            var tempList = EditorConfigUtil.LoadConfig<List<AssetBundleItemVO>>(CONFIG_NAME);
            //转成字典，方便查询
            Dictionary<string, AssetBundleItemVO> dic = new Dictionary<string, AssetBundleItemVO>();
            if (null != tempList)
            {
                foreach (var vo in tempList)
                {
                    dic.Add(vo.assetbundle, vo);
                }
            }
            return dic;
        }

        [HideLabel, DisplayAsString, ShowIf("_isFindingAB")]
        public string findTip = "正在同步AssetBundle数据......";

        bool _isFindingAB = false;

        [LabelText("保存配置"), Button(size: ButtonSizes.Large), PropertyOrder(-1)]
        void SaveConfig()
        {
            EditorConfigUtil.SaveConfig(abList, CONFIG_NAME);
            //this.ShowTip("保存完毕");
        }

        [PropertySpace(10)]
        [LabelText("开始生成"), Button(size: ButtonSizes.Large), PropertyOrder(-1)]
        void GeneratedAssetBundleNameClass()
        {
            var dir = Directory.GetParent(OUTPUT_CLASS_FILE);
            if (false == dir.Exists)
            {
                dir.Create();
            }

            var template = File.ReadAllText(TEMPLATE_FILE);
            StringBuilder sb = new StringBuilder();

            foreach (var vo in abList)
            {
                sb.AppendLine();
                sb.AppendLine();
                if (false == string.IsNullOrEmpty(vo.explain))
                {
                    sb.AppendLine(string.Format(FIELD_EXPLAIN_FORMAT, vo.explain));
                }
                sb.Append(string.Format(FIELD_FORMAT, vo.GetFieldName(), vo.assetbundle));
            }

            var classContent = template.Replace("{0}", sb.ToString());
            File.WriteAllText(OUTPUT_CLASS_FILE, classContent);


            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(OUTPUT_CLASS_FILE, 0);

            //this.ShowTip("生成完毕");
        }

        [Space(10)]
        [ShowInInspector]
        [LabelText("AssetBundle List"), ListDrawerSettings(IsReadOnly = true, Expanded = true), HideIf("_isFindingAB")]
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
                return assetbundle.Replace("/", "_").Replace(".ab", "").ToUpper();
            }
        }

        /// <summary>
        /// 找出所有要打包的资源
        /// </summary>
        void FindAssetBundles()
        {
            _isFindingAB = true;

            List<AssetBundleItemVO> list = new List<AssetBundleItemVO>();
            string[] dirs = Directory.GetDirectories(ZeroConst.HOT_RESOURCES_ROOT_DIR, "*", SearchOption.AllDirectories);
            foreach (var dir in dirs)
            {
                var sDir = FileSystem.StandardizeBackslashSeparator(dir);
                var di = new DirectoryInfo(sDir);
                if (di.GetFiles().Length == 0)
                {
                    continue;
                }

                string abName = sDir.Substring(ZeroConst.HOT_RESOURCES_ROOT_DIR.Length + 1) + ZeroConst.AB_EXTENSION;

                AssetBundleItemVO vo;
                vo.explain = _lastFindDic.ContainsKey(abName) ? _lastFindDic[abName].explain : "";
                vo.assetbundle = abName;
                list.Add(vo);
            }


            abList = list;


            _isFindingAB = false;
        }
    }
}
