using Jing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Zero
{
    public class AssetBundleResMgr : AResMgr
    {
        /// <summary>
        /// 资源描述
        /// </summary>
        AssetBundleManifest _manifest;

        /// <summary>
        /// 已加载的AB资源字典
        /// </summary>
        Dictionary<string, AssetBundle> _loadedABDic;

        public AssetBundleResMgr(string manifestFilePath)
        {
            UnloadAll();
            _loadedABDic = new Dictionary<string, AssetBundle>();

            RootDir = FileSystem.StandardizeBackslashSeparator(Path.GetDirectoryName(manifestFilePath));           

            AssetBundle ab = AssetBundle.LoadFromFile(manifestFilePath);
            _manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            if (_manifest == null)
            {
                throw new Exception(string.Format("错误的 Manifest File: {0}", manifestFilePath));
            }
            ab.Unload(false);
        }

        /// <summary>
        /// 让已加载的AB资源字典继承源资源管理器
        /// </summary>
        /// <param name="source"></param>
        internal void Inherit(AssetBundleResMgr source)
        {
            _loadedABDic = source._loadedABDic;
        }

        /// <summary>
        /// 如果
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        void MakeABNameNotEmpty(ref string abName)
        {
            if (string.IsNullOrEmpty(abName))
            {
                abName = ZeroConst.ROOT_AB_FILE_NAME;
            }            
        }

        public override string[] GetDepends(string abName)
        {
            MakeABNameNotEmpty(ref abName);
            abName = ABNameWithExtension(abName);
            string[] dependList = _manifest.GetAllDependencies(abName);
            return dependList;
        }

        public override T Load<T>(string abName, string assetName)
        {
            MakeABNameNotEmpty(ref abName);
            abName = ABNameWithExtension(abName);
            AssetBundle ab = LoadAssetBundle(abName);
            T asset = ab.LoadAsset<T>(assetName);
            if (null == asset)
            {
                Log.E("获取的资源不存在： AssetBundle: {0}  Asset: {1}", abName, assetName);
            }
            return asset;
        }

        public override void LoadAsync(string abName, string assetName, Action<UnityEngine.Object> onLoaded, Action<float> onProgress = null)
        {
            MakeABNameNotEmpty(ref abName);
            abName = ABNameWithExtension(abName);
            AssetBundle ab = LoadAssetBundle(abName);
            ILBridge.Ins.StartCoroutine(LoadAsync(ab, assetName, onLoaded, onProgress));
        }

        IEnumerator LoadAsync(AssetBundle ab, string assetName, Action<UnityEngine.Object> onLoaded, Action<float> onProgress)
        {
            AssetBundleRequest abr = ab.LoadAssetAsync<GameObject>(assetName);

            do
            {
                if (onProgress != null)
                {
                    onProgress.Invoke(abr.progress);
                }
                yield return new WaitForEndOfFrame();
            }
            while (false == abr.isDone);

            //加载完成
            onLoaded.Invoke(abr.asset);
        }

        public override void Unload(string abName, bool isUnloadAllLoaded = false, bool isUnloadDepends = true)
        {
            MakeABNameNotEmpty(ref abName);
            abName = ABNameWithExtension(abName);
            if (_loadedABDic.ContainsKey(abName))
            {
                AssetBundle ab = _loadedABDic[abName];
                _loadedABDic.Remove(abName);
                ab.Unload(isUnloadAllLoaded);
                //Debug.LogFormat("释放AB：{0}", abName);

                if (isUnloadDepends)
                {
                    string[] dependList = _manifest.GetAllDependencies(abName);
                    foreach (string depend in dependList)
                    {
                        if (false == CheckDependencies(depend))
                        {
                            Unload(depend, isUnloadAllLoaded, isUnloadDepends);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 检查ab资源是否被已加载的资源依赖
        /// </summary>
        /// <param name="ab"></param>
        /// <param name="depend"></param>
        /// <returns></returns>
        bool CheckDependencies(string ab)
        {
            foreach(var loadedEntry in _loadedABDic)
            {
                var entryDepends = _manifest.GetAllDependencies(loadedEntry.Key);
                foreach(var entryDepend in entryDepends)
                {
                    if(ab == entryDepend)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void UnloadAll(bool isUnloadAllLoaded = false)
        {
            if (null != _loadedABDic)
            {
                foreach (AssetBundle cached in _loadedABDic.Values)
                {
                    cached.Unload(isUnloadAllLoaded);
                }
                _loadedABDic.Clear();
            }

            ResMgr.Ins.DoGC();
        }

        /// <summary>
        /// 加载AB包，自动处理依赖问题
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        private AssetBundle LoadAssetBundle(string abName)
        {
            MakeABNameNotEmpty(ref abName);
            abName = ABNameWithExtension(abName);
            string abPath = FileSystem.CombinePaths(RootDir, abName);
            if (false == File.Exists(abPath))
            {
                //加载的AB资源不存在
                Log.E(string.Format("[{0}] 不存在", abPath));
                return null;                
            }

            //依赖检查
            string[] dependList = _manifest.GetAllDependencies(abName);
            foreach (string depend in dependList)
            {
                //string dependPath = Path.Combine(_rootDir, depend);
                if (false == _loadedABDic.ContainsKey(depend))
                {
                    _loadedABDic[depend] = LoadAssetBundle(depend);
                }
            }

            AssetBundle ab = null;
            if (_loadedABDic.ContainsKey(abName))
            {
                ab = _loadedABDic[abName];
            }
            else
            {                
                ab = AssetBundle.LoadFromFile(abPath);
                _loadedABDic[abName] = ab;
            }
            return ab;
        }
    }
}
