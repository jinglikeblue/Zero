using Jing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    public class AssetBundleBuilder
    {
        public const string EXT = ".ab";
        string _scanStartAssetDir;
        string _outPath;
        string _searchStartPath;
        string _assetDirPath;
        string _rootABName;

        /// <summary>
        /// key：ab包名称   value：对应的资源列表
        /// </summary>
        Dictionary<string, List<string>> _abDic = new Dictionary<string, List<string>>();

        /// <summary>
        /// key: ab包名称   value：ab包依赖的资源的Set集合
        /// </summary>
        Dictionary<string, HashSet<string>> _dependsDic = new Dictionary<string, HashSet<string>>();

        public AssetBundleBuilder(string outPath, string scanStartAssetDir)
        {            
            _outPath = outPath;
            _scanStartAssetDir = scanStartAssetDir.Replace("\\", "/");
            _assetDirPath = Path.GetDirectoryName(Application.dataPath) + "/";
            _assetDirPath = _assetDirPath.Replace("\\", "/");
        }

        public void Start()
        {
            EditorUtility.DisplayProgressBar("正在开始...", "即将开始", 0f);
            if (false == Directory.Exists(_outPath))
            {
                Directory.CreateDirectory(_outPath);
            }

            string searchStartPath = Path.Combine(_assetDirPath, _scanStartAssetDir);
            //searchStartPath = searchStartPath.Replace("\\", "/");
            if (false == Directory.Exists(searchStartPath))
            {
                return;
            }

            _rootABName = Path.GetFileName(Path.GetDirectoryName(searchStartPath)) + EXT;
            
            //找出所有打了标记的资源
            FindAB(searchStartPath);
            //根据依赖资源分析构建交叉引用AB
            BestCrossFind();            
            //打包AB资源
            Publish();
            //给Manifest包加上后缀
            AddExt2ManifestFile();

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        void AddExt2ManifestFile()
        {
            string file = Path.GetFileName(_outPath);
            string oldFile = Path.Combine(_outPath, file);
            string newFile = FileSystem.CombinePaths(_outPath, string.Format("manifest{0}", EXT));
            if(File.Exists(newFile))
            {
                File.Delete(newFile);
            }
            FileUtil.MoveFileOrDirectory(oldFile, newFile);

            string oldTxtFile = oldFile + ".manifest";
            string newTxtFile = string.Format("{0}/manifest.manifest", _outPath);
            if (File.Exists(newTxtFile))
            {
                File.Delete(newTxtFile);
            }
            FileUtil.MoveFileOrDirectory(oldTxtFile, newTxtFile);
        }

        void FindAB(string dir)
        {
            string[] assetFileList = Directory.GetFiles(dir);
            foreach (string assetFile in assetFileList)
            {
                var temp = assetFile.Replace("\\", "/").Replace(_assetDirPath, "");
                AssetImporter ai = AssetImporter.GetAtPath(temp);

                if (null == ai || ai.assetBundleName == null || ai.assetBundleName.Length == 0)
                {
                    continue;
                }

                //根据资源的路径分AB包                 
                string assetPath = ai.assetPath.Replace(_scanStartAssetDir, "");
                string abName = Path.GetDirectoryName(assetPath) + EXT;
                if(abName == EXT)
                {
                    //资源直接在根目录下
                    abName = _rootABName;
                }
                GetAssetList(abName).Add(ai.assetPath);
                //找出依赖资源
                FindDepends(ai, GetDependsSet(abName));
            }

            string[] subDirs = Directory.GetDirectories(dir);
            foreach (string subDir in subDirs)
            {
                FindAB(subDir);
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
                if (dependPath.Contains(ai.assetPath) || dependPath.Contains(".cs"))
                {
                    //要过滤掉依赖的自己本身和脚本文件，自己本身的名称已设置，而脚本不能打包
                    continue;
                }

                AssetImporter dependAI = AssetImporter.GetAtPath(dependPath);
                if (dependAI.assetBundleName != "")
                {
                    //已经指定到AB的不做处理
                    continue;
                }

                //依赖的资源
                if (false == dependsSet.Contains(dependPath))
                {
                    dependsSet.Add(dependPath);
                }
            }
        }

        #region 如果BestCrossFind()没有BUG的话，该代码可在之后删除
        /// <summary>
        /// 简单的计算，将所有产生的依赖放到一个文件中
        /// </summary>
        //void SimpleCrossFind()
        //{
        //    HashSet<string> crossResult = new HashSet<string>();
        //    HashSet<string> dependsTotalSet = new HashSet<string>();
        //    foreach (var entry in _dependsDic)
        //    {
        //        foreach (var dependPath in entry.Value)
        //        {
        //            if (false == dependsTotalSet.Contains(dependPath))
        //            {
        //                //第一次出现的资源，记录
        //                dependsTotalSet.Add(dependPath);
        //            }
        //            else
        //            {
        //                //第二次出现，表示重复依赖，如果没有添加到交叉列表，则添加                        
        //                crossResult.Add(dependPath);
        //            }
        //        }
        //    }

        //    if (crossResult.Count > 0)
        //    {
        //        _abDic.Add("auto_depends/cross" + EXT, crossResult.ToList<string>());
        //    }
        //}
        #endregion

        /// <summary>
        /// 最佳化计算，依赖的ab包的数量会增多，但是保持的是项目的最小颗粒化依赖
        /// </summary>
        void BestCrossFind()
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
                string abName = string.Format("auto_depends/cross_{0}{1}", i++, EXT);
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

        private void Publish()
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

            AssetBundleManifest abm = BuildPipeline.BuildAssetBundles(_outPath, abbList, BuildAssetBundleOptions.ChunkBasedCompression, ZeroEditorUtil.CurrentPlatform);
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
    }
}
