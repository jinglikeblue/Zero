using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Zero
{
    class AssetBundleResMgr : AResMgr
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

            RootDir = Path.GetDirectoryName(manifestFilePath);

            AssetBundle ab = AssetBundle.LoadFromFile(manifestFilePath);
            _manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            if (_manifest == null)
            {
                throw new Exception(string.Format("错误的 Manifest File: {0}", manifestFilePath));
            }
            ab.Unload(false);
        }

        public override string[] GetDepends(string abName)
        {
            string[] dependList = _manifest.GetAllDependencies(abName);
            return dependList;
        }

        public override T Load<T>(string abName, string assetName)
        {
            AssetBundle ab = LoadAssetBundle(abName);
            T asset = ab.LoadAsset<T>(assetName);
            return asset;
        }

        public override void LoadAsync(string abName, string assetName, Action<UnityEngine.Object> onLoaded, Action<float> onProgress = null)
        {
            AssetBundle ab = LoadAssetBundle(abName);
            CoroutineBridge.Ins.StartCoroutine(LoadAsync(ab, assetName, onLoaded, onProgress));
        }

        public override void Unload(string abName, bool isUnloadAllLoaded = false, bool isUnloadDepends = true)
        {
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
                        Unload(depend, isUnloadAllLoaded, isUnloadDepends);
                    }
                }
            }
        }

        public override void UnloadAll(bool isUnloadAllLoaded = false)
        {
            if (null != _loadedABDic)
            {
                foreach (AssetBundle cached in _loadedABDic.Values)
                {
                    cached.Unload(isUnloadAllLoaded);
                }
            }
        }

        IEnumerator LoadAsync(AssetBundle ab, string assetName, Action<UnityEngine.Object> onGet, Action<float> onProgress)
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
            onGet.Invoke(abr.asset);
        }

        /// <summary>
        /// 加载AB包，自动处理依赖问题
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        private AssetBundle LoadAssetBundle(string abName)
        {
            string abPath = Path.Combine(RootDir, abName);
            if (false == File.Exists(abPath))
            {
                //加载的AB资源不存在
                throw new Exception(string.Format("[{0}] 不存在", abPath));
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
