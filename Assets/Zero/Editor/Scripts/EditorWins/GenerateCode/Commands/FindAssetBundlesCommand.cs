using Jing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Zero;

namespace ZeroEditor
{
    /// <summary>
    /// 查找AB资源信息
    /// </summary>
    public class FindAssetBundlesCommand
    {
        /// <summary>
        /// 查找完成事件(异步事件)
        /// </summary>
        public event Action<FindAssetBundlesCommand, List<AssetBundleItemVO>> onFinished;

        /// <summary>
        /// 配置
        /// </summary>
        public GenerateAssetBundleNameModule.ConfigVO cfg { get; private set; }

        /// <summary>
        /// 查到的AssetBundle列表
        /// </summary>
        public List<AssetBundleItemVO> list { get; private set; }

        bool _isAsync;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isAsync">是否异步执行cmd</param>
        public FindAssetBundlesCommand(bool isAsync = true)
        {
            _isAsync = isAsync;
        }

        public void Excute()
        {
            cfg = GenerateAssetBundleNameModule.LoadConfig();
            if (_isAsync)
            {
                new Thread(FindABAsync).Start();
            }
            else
            {
                FindABAsync();
            }            
        }

        void FindABAsync()
        {
            //查找当前@Resources的AB信息
            List<AssetBundleItemVO>  list = SeanHotResources();

            //配置中的数据，转成字典
            Dictionary<string, AssetBundleItemVO> dic = new Dictionary<string, AssetBundleItemVO>();
            if (null != cfg.abList)
            {
                foreach (var vo in cfg.abList)
                {
                    dic.Add(vo.assetbundle, vo);
                }
            }

            for(int i = 0; i < list.Count; i++)
            {
                var ab = list[i];
                if (dic.ContainsKey(ab.assetbundle))
                {
                    ab.explain = dic[ab.assetbundle].explain;
                    list[i] = ab;
                }                
            }

            this.list = list;

            onFinished?.Invoke(this, this.list);
        }

        List<AssetBundleItemVO> SeanHotResources()
        {
            List<AssetBundleItemVO> list = new List<AssetBundleItemVO>();

            //添加默认的
            var rootFiles = Directory.GetFiles(ZeroConst.HOT_RESOURCES_ROOT_DIR);
            if (rootFiles.Length > 0)
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
                if (rootItem.assetList.Count > 0)
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
                vo.explain = null;
                //vo.explain = _lastFindDic.ContainsKey(abName) ? _lastFindDic[abName].explain : "";
                vo.assetbundle = abName;
                vo.assetList = new List<string>();
                foreach (var file in files)
                {
                    if (file.Extension.Equals(".meta"))
                    {
                        continue;
                    }
                    vo.assetList.Add(file.Name);
                }
                list.Add(vo);
            }
            return list;
        }
    }
}

