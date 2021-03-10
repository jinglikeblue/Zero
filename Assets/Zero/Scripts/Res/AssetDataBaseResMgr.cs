using Jing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zero;

namespace Zero
{
    /// <summary>
    /// 该资源管理器仅适用于Editor下的开发调试
    /// </summary>
    class AssetDataBaseResMgr : AResMgr
    {
        string _assetRoot;

        public AssetDataBaseResMgr(string assetRoot)
        {
#if !UNITY_EDITOR
        throw new Exception("AssetDataBaseResMgr仅在Editor模式下可用");
#endif
            _assetRoot = assetRoot;
        }

        string GetDirOfAB(string abName)
        {
            abName = ABNameWithoutExtension(abName);
            string dir;
            if (abName.ToLower() != ZeroConst.ROOT_AB_FILE_NAME) //resources表示从根目录获取资源，则不需要添加目录
            {
                dir = FileUtility.CombinePaths(_assetRoot, abName);
            }
            else
            {
                dir = FileUtility.CombinePaths(_assetRoot);
            }
            return dir;
        }

        /// <summary>
        /// 将AB资源的路径格式转换为Resources目录中资源的路径
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        string AssetBundlePath2ResourcePath(string abName, string assetName)
        {
            try
            {                
                string dir = GetDirOfAB(abName);

                //模糊匹配资源名称
                var files = Directory.GetFiles(dir);

                bool isAssetNameContainExt = assetName.Contains(".");

                foreach (var file in files)
                {
                    if (Path.GetExtension(file) == ".meta")
                    {
                        continue;
                    }

                    if (isAssetNameContainExt && Path.GetFileName(file) == assetName)
                    {
                        return file;
                    }
                    else if(Path.GetFileNameWithoutExtension(file) == assetName)
                    {
                        return file;
                    }
                }
            }
            catch
            {
                throw new Exception(string.Format("在[{0}]下无法找到资源文件[{1}/{2}]", _assetRoot, ABNameWithoutExtension(abName), assetName));
            }
            return null;
        }

        public override string[] GetDepends(string abName)
        {
            return new string[0];
        }

        public override string[] GetAllAsssetsNames(string abName)
        {
#if UNITY_EDITOR

            var assetNames = new List<string>();

            var dirPath = GetDirOfAB(abName);

            //模糊匹配资源名称
            var files = Directory.GetFiles(dirPath);

            foreach (var file in files)
            {
                var filePath = FileUtility.StandardizeBackslashSeparator(file);
                if (Path.GetExtension(filePath) == ".meta")
                {
                    continue;
                }

                if (File.Exists(filePath + ".meta"))
                {
                    assetNames.Add(Path.GetFileName(filePath));
                }
            }

            return assetNames.Count > 0 ? assetNames.ToArray() : null;
#else
        return null;
#endif
        }

        public override UnityEngine.Object Load(string abName, string assetName)
        {
            return Load<UnityEngine.Object>(abName, assetName);
        }

        public override T Load<T>(string abName, string assetName)
        {
#if UNITY_EDITOR            
            string path = AssetBundlePath2ResourcePath(abName, assetName);
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);            
            if (null == asset)
            {
                Debug.LogErrorFormat("资源不存在：{0}", ResMgr.Ins.LinkAssetPath(abName, assetName));
            }
            return asset;
#else
        return default(T);
#endif
        }

        public override void LoadAsync(string abName, string assetName, Action<UnityEngine.Object> onLoaded, Action<float> onProgress = null)
        {
            ILBridge.Ins.StartCoroutine(ResourceLoadAsync<UnityEngine.Object>(AssetBundlePath2ResourcePath(abName, assetName), onLoaded, onProgress));
        }

        public override void LoadAsync<T>(string abName, string assetName, Action<T> onLoaded, Action<float> onProgress = null)
        {
            ILBridge.Ins.StartCoroutine(ResourceLoadAsync<T>(AssetBundlePath2ResourcePath(abName, assetName), onLoaded, onProgress));
        }

        IEnumerator ResourceLoadAsync<T>(string assetPath, Action<T> onLoaded, Action<float> onProgress) where T : UnityEngine.Object
        {
            if (null != onProgress)
            {
                onProgress.Invoke(0);
            }
            yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
            if (null != onProgress)
            {
                onProgress.Invoke(1);
            }
            T obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);

            if(null == obj)
            {
                Debug.LogErrorFormat("资源并不存在: {0}", assetPath);                
            }

            if (null != onLoaded)
            {
                onLoaded.Invoke(obj);
            }
#else
            onLoaded?.Invoke(null);
#endif
        }

        public override void Unload(string abName, bool isUnloadAllLoaded = false, bool isUnloadDepends = true)
        {
            Resources.UnloadUnusedAssets();
        }

        public override void UnloadAll(bool isUnloadAllLoaded = false)
        {
            Resources.UnloadUnusedAssets();
        }



        public override UnityEngine.Object[] LoadAll(string abName)
        {
#if UNITY_EDITOR

            var assets = new List<UnityEngine.Object>();

            var dirPath = GetDirOfAB(abName);

            //模糊匹配资源名称
            var files = Directory.GetFiles(dirPath);            

            foreach (var file in files)
            {
                var filePath = FileUtility.StandardizeBackslashSeparator(file);
                if (Path.GetExtension(filePath) == ".meta")
                {
                    continue;
                }
                
                if(File.Exists(filePath + ".meta"))
                {
                    var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(filePath);
                    assets.Add(asset);
                }                
            }

            return assets.Count > 0 ? assets.ToArray() : null;
#else
        return null;
#endif
        }

        public override void LoadAllAsync(string abName, Action<UnityEngine.Object[]> onLoaded, Action<float> onProgress = null)
        {
            ILBridge.Ins.StartCoroutine(ResourceLoadAllAsync(abName, onLoaded, onProgress));
        }

        IEnumerator ResourceLoadAllAsync(string abName, Action<UnityEngine.Object[]> onLoaded, Action<float> onProgress)
        {
            if (null != onProgress)
            {
                onProgress.Invoke(0);
            }
            yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
            if (null != onProgress)
            {
                onProgress.Invoke(1);
            }
            var assets = LoadAll(abName);

            onLoaded?.Invoke(assets); 
#else
            onLoaded?.Invoke(null);
#endif
        }


    }
}