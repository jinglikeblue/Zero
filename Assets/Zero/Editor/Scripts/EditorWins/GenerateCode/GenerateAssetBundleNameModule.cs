using Jing;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    public class GenerateAssetBundleNameModule: AEditorModule
    {
        /// <summary>
        /// 配置文件位置
        /// </summary>
        const string CONFIG_NAME = "asset_bundle_name_config.json";

        Dictionary<string, AssetBundleItemVO> _lastFindDic;

        struct ConfigVO {
            public List<AssetBundleItemVO> abList;
            public string viewClassNS;
        }

        ConfigVO cfg;

        public GenerateAssetBundleNameModule(EditorWindow win) : base(win)
        {
            _lastFindDic = LoadConfig();
            abList = cfg.abList;
            viewClassNS = cfg.viewClassNS;
            new Thread(FindAssetBundles).Start();
        }

        Dictionary<string, AssetBundleItemVO> LoadConfig()
        {
            try
            {
                cfg = EditorConfigUtil.LoadConfig<ConfigVO>(CONFIG_NAME);
            }
            catch(Exception e)
            {
                Debug.LogError(e);
                cfg = new ConfigVO();
            }
            //转成字典，方便查询
            Dictionary<string, AssetBundleItemVO> dic = new Dictionary<string, AssetBundleItemVO>();
            if (null != cfg.abList)
            {
                foreach (var vo in cfg.abList)
                {
                    dic.Add(vo.assetbundle, vo);
                }
            }
            return dic;
        }

        [HideLabel, DisplayAsString, ShowIf("_isFindingAB")]
        public string findTip = "正在同步AssetBundle数据......";

        bool _isFindingAB = false;

        [Title("资源相关代码生成", TitleAlignment = TitleAlignments.Centered)]
        [LabelText("保存配置"), Button(size: ButtonSizes.Large), PropertyOrder(-1)]
        void SaveConfig()
        {
            cfg.viewClassNS = viewClassNS;
            cfg.abList = abList;
            EditorConfigUtil.SaveConfig(cfg, CONFIG_NAME);
            //this.ShowTip("保存完毕");
        }

        [PropertySpace(10)]
        [LabelText("开始生成"), Button(size: ButtonSizes.Large), PropertyOrder(-1)]
        void GeneratedAssetBundleNameClass()
        {
            new GenerateABClassCommand(abList).Excute();
            new GenerateAutoViewRegisterClassCommand(abList, viewClassNS).Excute();
            editorWin.ShowTip("生成完毕!");
        }   

        [Space(10)]
        [LabelText("资源名称类生成地址")]
        [DisplayAsString]
        [InlineButton("OpenABClassFile", "Open")]
        public string abClassPath = GenerateABClassCommand.OUTPUT_FILE;

        [LabelText("视图类命名空间前缀(以\".\"结尾)")]
        public string viewClassNS;
        
        [LabelText("视图注册类生成地址")]
        [DisplayAsString]
        [InlineButton("OpenRegisterClassFile", "Open")]
        public string registerClassPath = GenerateAutoViewRegisterClassCommand.OUTPUT_FILE;

        void OpenABClassFile()
        {
            OpenFile(GenerateABClassCommand.OUTPUT_FILE);
        }

        void OpenRegisterClassFile()
        {
            OpenFile(GenerateAutoViewRegisterClassCommand.OUTPUT_FILE);
        }

        void OpenFile(string file)
        {
            if (File.Exists(file))
            {
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(file, 0);
            }
            else
            {
                editorWin.ShowTip("文件不存在!");
            }
        }

        [Space(10)]
        [ShowInInspector]
        [LabelText("AssetBundle List"), ListDrawerSettings(IsReadOnly = true, Expanded = true, NumberOfItemsPerPage = 6), HideIf("_isFindingAB")]
        public List<AssetBundleItemVO> abList;

        /// <summary>
        /// 找出所有要打包的资源
        /// </summary>
        void FindAssetBundles()
        {
            _isFindingAB = true;

            List<AssetBundleItemVO> list = new List<AssetBundleItemVO>();

            //添加默认的
            var rootFiles = Directory.GetFiles(ZeroConst.HOT_RESOURCES_ROOT_DIR);
            if(rootFiles.Length > 0)
            {
                AssetBundleItemVO rootItem;
                rootItem.explain = @"@Resources根目录下资源";
                rootItem.assetbundle = ZeroConst.ROOT_AB_FILE_NAME + ZeroConst.AB_EXTENSION;
                rootItem.assetList = new List<string>();
                foreach (var file in rootFiles)
                {
                    if (Path.GetExtension(file).Equals(".meta"))
                    {
                        continue;
                    }
                    rootItem.assetList.Add(Path.GetFileName(file));
                }
                if(rootItem.assetList.Count > 0)
                {
                    list.Add(rootItem);
                }
            }

            string[] dirs = Directory.GetDirectories(ZeroConst.HOT_RESOURCES_ROOT_DIR, "*", SearchOption.AllDirectories);
            foreach (var dir in dirs)
            {
                var sDir = FileSystem.StandardizeBackslashSeparator(dir);
                var di = new DirectoryInfo(sDir);
                var files = di.GetFiles();
                if (di.GetFiles().Length == 0)
                {
                    continue;
                }

                string abName = sDir.Substring(ZeroConst.HOT_RESOURCES_ROOT_DIR.Length + 1) + ZeroConst.AB_EXTENSION;

                AssetBundleItemVO vo;
                vo.explain = _lastFindDic.ContainsKey(abName) ? _lastFindDic[abName].explain : "";
                vo.assetbundle = abName;
                vo.assetList = new List<string>();
                foreach(var file in files)
                {
                    if (file.Extension.Equals(".meta"))
                    {
                        continue;
                    }
                    vo.assetList.Add(file.Name);
                }
                list.Add(vo);                               
            }

            abList = list;
            _isFindingAB = false;
        }
    }
}
