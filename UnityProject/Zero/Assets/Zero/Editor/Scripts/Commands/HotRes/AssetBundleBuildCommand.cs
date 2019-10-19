using Jing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    /// <summary>
    /// AssetBundle打包指令
    /// </summary>
    class AssetBundleBuildCommand
    {
        /// <summary>
        /// 资源的根目录
        /// </summary>
        public readonly string resRootDir;

        /// <summary>
        /// 资源的输出
        /// </summary>
        public readonly string outputDir;

        /// <summary>
        /// 发布生成的Manifest文件
        /// </summary>
        public AssetBundleManifest assetBundleManifest { get; private set; }

        /// <summary>
        /// key：ab包名称   value：对应的资源列表
        /// </summary>
        Dictionary<string, List<string>> _abDic;

        /// <summary>
        /// key: ab包名称   value：ab包依赖的资源的Set集合
        /// </summary>
        Dictionary<string, HashSet<string>> _dependsDic;


        public AssetBundleBuildCommand(string resRootDir, string outputDir)
        {
            this.resRootDir = FileSystem.CombineDirs(true, resRootDir);
            this.outputDir = outputDir;
            _abDic = new Dictionary<string, List<string>>();
            _dependsDic = new Dictionary<string, HashSet<string>>();
        }

        public void Execute()
        {
            //找出所有要打包的资源
            FindAssetBundles();

            //根据交叉引用算法优化AssetBundle
            CreateCrossAssetBundle();

            //开始打包AssetBundle（打包到Library中的ZeroHotResCache中）
            BuildAssetBundlesToCacheDir();

            //从「ZeroHotResCache」中拷贝需要的文件到resDir中
            Move2ReleaseDir();

        }

        /// <summary>
        /// 找出所有要打包的资源
        /// </summary>
        private void FindAssetBundles()
        {
            string[] assetFileList = Directory.GetFiles(resRootDir, "*", SearchOption.AllDirectories);
            foreach (string assetFile in assetFileList)
            {
                if (Path.GetExtension(assetFile).Equals(".meta"))
                {
                    continue;
                }

                var temp = FileSystem.StandardizeBackslashSeparator(assetFile);
                AssetImporter ai = AssetImporter.GetAtPath(temp);

                if (null == ai)
                {
                    continue;
                }

                //根据资源的路径分AB包                 
                string assetPath = ai.assetPath.Replace(resRootDir, "");
                //根据资源所在文件夹，计算AssetBundle文件名
                string abName = Path.GetDirectoryName(assetPath);                
                if (string.IsNullOrEmpty(abName))
                {
                    //资源直接在根目录下
                    abName = ZeroConst.ROOT_AB_FILE_NAME;
                }
                else
                {
                    abName = FileSystem.StandardizeBackslashSeparator(abName);
                }

                //加上后缀名
                abName += ZeroConst.AB_EXTENSION;

                GetAssetList(abName).Add(ai.assetPath);
                //找出依赖资源
                FindDepends(ai, GetDependsSet(abName));
            }
        }

        /// <summary>
        /// 找出资源依赖的资源（如果依赖的资源已标记为AB，则忽略）
        /// </summary>
        /// <param name="ai"></param>
        void FindDepends(AssetImporter ai, HashSet<string> dependsSet)
        {
            //获取依赖的资源
            string[] dps = AssetDatabase.GetDependencies(ai.assetPath);
            foreach (string dependPath in dps)
            {
                if (dependPath.StartsWith(resRootDir) || dependPath.Contains(".cs"))
                {
                    //要过滤掉依赖的@Resources目录中的文件和脚本文件，@Resources目录中的文件已设置，而脚本不能打包
                    continue;
                }

                //依赖的资源
                if (false == dependsSet.Contains(dependPath))
                {
                    dependsSet.Add(dependPath);
                }
            }
        }

        List<string> GetAssetList(string abName)
        {
            if (false == _abDic.ContainsKey(abName))
            {
                _abDic[abName] = new List<string>();
            }

            return _abDic[abName];
        }

        HashSet<string> GetDependsSet(string abName)
        {
            if (false == _dependsDic.ContainsKey(abName))
            {
                _dependsDic[abName] = new HashSet<string>();
            }

            return _dependsDic[abName];
        }

        /// <summary>
        /// 创建交叉文件资源。最佳化计算，依赖的ab包的数量会增多，但是保持的是项目的最小颗粒化依赖
        /// </summary>
        void CreateCrossAssetBundle()
        {
            #region 找出每一个资源依赖它的AB集合Set
            Dictionary<string, HashSet<string>> asset2ABDic = new Dictionary<string, HashSet<string>>();
            foreach (var ab in _dependsDic)
            {
                foreach (var asset in ab.Value)
                {
                    if (false == asset2ABDic.ContainsKey(asset))
                    {
                        asset2ABDic[asset] = new HashSet<string>();
                    }
                    asset2ABDic[asset].Add(ab.Key);
                }
            }
            #endregion

            #region 移除掉只被一个AB依赖的资源，这种资源只需要和唯一依赖它的AB一起打包就行了
            HashSet<string> toRemoveKeySet = new HashSet<string>();
            foreach (var pair in asset2ABDic)
            {
                if (pair.Value.Count <= 1)
                {
                    toRemoveKeySet.Add(pair.Key);
                }
            }

            foreach (var toRemoveKey in toRemoveKeySet)
            {
                asset2ABDic.Remove(toRemoveKey);
            }
            #endregion

            #region 遍历每一个资源，找到和它被同样AB集合的资源，打到一个依赖AB包中，依次命名为cross_0, cross_1, cross_2
            int i = 0;
            //标记已处理的资源
            HashSet<string> usedAsset = new HashSet<string>();
            foreach (var assetPair in asset2ABDic)
            {
                if (usedAsset.Contains(assetPair.Key))
                {
                    continue;
                }
                string abName = string.Format("auto_depends/cross_{0}{1}", i++, ZeroConst.AB_EXTENSION);
                List<string> assetList = new List<string>();
                assetList.Add(assetPair.Key);
                //标记为已使用
                usedAsset.Add(assetPair.Key);
                foreach (var assetPair1 in asset2ABDic)
                {
                    if (usedAsset.Contains(assetPair1.Key))
                    {
                        continue;
                    }

                    //首先判断被AB依赖的数量是否一致
                    if (assetPair.Value.Count == assetPair1.Value.Count)
                    {
                        bool isSame = true;
                        //判断是否所有的AB都一样
                        foreach (var tempABName in assetPair.Value)
                        {
                            if (false == assetPair1.Value.Contains(tempABName))
                            {
                                isSame = false;
                                break;
                            }
                        }

                        if (isSame)
                        {
                            assetList.Add(assetPair1.Key);
                            usedAsset.Add(assetPair1.Key);
                        }
                    }
                }

                _abDic[abName] = assetList;
            }
            #endregion
        }

        /// <summary>
        /// 首先打包AssetBundle到缓存发布目录
        /// </summary>
        void BuildAssetBundlesToCacheDir()
        {
            AssetBundleBuild[] abbList = new AssetBundleBuild[_abDic.Count];
            int i = 0;
            foreach (var abb in _abDic)
            {
                abbList[i] = new AssetBundleBuild();
                abbList[i].assetBundleName = abb.Key;
                abbList[i].assetNames = abb.Value.ToArray();
                i++;
            }

            if (false == Directory.Exists(ZeroEditorConst.ASSET_BUNDLE_CACHE_DIR))
            {
                Directory.CreateDirectory(ZeroEditorConst.ASSET_BUNDLE_CACHE_DIR);
            }

            assetBundleManifest = BuildPipeline.BuildAssetBundles(ZeroEditorConst.ASSET_BUNDLE_CACHE_DIR, abbList, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle, ZeroEditorConst.BUILD_PLATFORM);

            //清理不再需要的资源(需要修改算法，只保留需要的，完全清理不需要的资源)
            CleanCahceDir();
        }

        void CleanCahceDir()
        {
            var manifest = assetBundleManifest;
            var assetBundles = manifest.GetAllAssetBundles();
            //需要留下的文件
            HashSet<string> usefulFileSet = new HashSet<string>();
            usefulFileSet.Add(ZeroConst.AB_DIR_NAME);
            usefulFileSet.Add(ZeroConst.AB_DIR_NAME + ".manifest");
            foreach (var abFile in assetBundles)
            {
                usefulFileSet.Add(abFile);
                usefulFileSet.Add(abFile + ".manifest");
            }

            var abDir = FileSystem.CombineDirs(true, ZeroEditorConst.ASSET_BUNDLE_CACHE_DIR);
            var files = Directory.GetFiles(ZeroEditorConst.ASSET_BUNDLE_CACHE_DIR, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var tempFile = FileSystem.StandardizeBackslashSeparator(file);
                var filePath = tempFile.Replace(abDir, "");
                if (false == usefulFileSet.Contains(filePath))
                {
                    //不需要的AB
                    File.Delete(tempFile);
                    Log.I("删除文件：" + tempFile);
                }
            }

            //删除空的文件夹
            DirectoryInfo dir = new DirectoryInfo(ZeroEditorConst.ASSET_BUNDLE_CACHE_DIR);
            DirectoryInfo[] subdirs = dir.GetDirectories("*.*", SearchOption.AllDirectories);
            foreach (DirectoryInfo subdir in subdirs)
            {
                if (false == Directory.Exists(subdir.FullName))
                {
                    continue;
                }
                FileInfo[] subFiles = subdir.GetFiles("*", SearchOption.AllDirectories);
                if (subFiles.Length == 0)
                {
                    subdir.Delete(true);
                    Log.I("删除文件夹：" + FileSystem.StandardizeBackslashSeparator(subdir.FullName));
                }
            }

            AssetBundle.UnloadAllAssetBundles(true);
        }

        void Move2ReleaseDir()
        {
            //清空发布目录
            if (Directory.Exists(outputDir))
            {
                Directory.Delete(outputDir, true);
            }

            //移动AB文件
            var assetBundles = assetBundleManifest.GetAllAssetBundles();
            foreach (var ab in assetBundles)
            {
                string sourceABPath = FileSystem.CombinePaths(ZeroEditorConst.ASSET_BUNDLE_CACHE_DIR, ab);
                string targetABPath = FileSystem.CombinePaths(outputDir, ab);
                FileSystem.Copy(sourceABPath, targetABPath, true);
            }

            //移动Manifest文件
            string sourceManifestPath = FileSystem.CombinePaths(ZeroEditorConst.ASSET_BUNDLE_CACHE_DIR, ZeroConst.AB_DIR_NAME);
            string targetManifestPath = FileSystem.CombinePaths(outputDir, ZeroConst.MANIFEST_FILE_NAME + ZeroConst.AB_EXTENSION);
            FileSystem.Copy(sourceManifestPath, targetManifestPath, true);
        }
    }    
}