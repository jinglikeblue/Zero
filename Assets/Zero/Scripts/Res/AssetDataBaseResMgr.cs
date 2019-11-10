using Jing;
using System;
using System.Collections;
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

        /// <summary>
        /// 将AB资源的路径格式转换为Resources目录中资源的路径
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        string AssetBundlePath2ResourcePath(string abName, string assetName)
        {
            try
            {
                abName = ABNameWithoutExtension(abName);
                string dir;
                if (abName.ToLower() != ZeroConst.ROOT_AB_FILE_NAME) //resources表示从根目录获取资源，则不需要添加目录
                {
                    dir = FileSystem.CombinePaths(_assetRoot, abName);
                }
                else
                {
                    dir = FileSystem.CombinePaths(_assetRoot);
                }

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
                throw new Exception(string.Format("在[{0}]下无法找到资源文件[{1}/{2}]", _assetRoot, abName, assetName));
            }
            return null;
        }

        public override string[] GetDepends(string abName)
        {
            return new string[0];
        }

        public override T Load<T>(string abName, string assetName)
        {
#if UNITY_EDITOR            
            string path = AssetBundlePath2ResourcePath(abName, assetName);
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
            if (null == asset)
            {
                Debug.LogErrorFormat("资源不存在：{0}", path);
            }
            return asset;
#else
        return default(T);
#endif
        }

        public override void LoadAsync(string abName, string assetName, Action<UnityEngine.Object> onLoaded, Action<float> onProgress = null)
        {
            ILBridge.Ins.StartCoroutine(ResourceLoadAsync(AssetBundlePath2ResourcePath(abName, assetName), onLoaded, onProgress));
        }

        IEnumerator ResourceLoadAsync(string assetPath, Action<UnityEngine.Object> onLoaded, Action<float> onProgress)
        {
            if(null != onProgress)
            {
                onProgress.Invoke(0);
            }            
            yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
            if (null != onProgress)
            {
                onProgress.Invoke(1);
            }
            UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
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
    }
}