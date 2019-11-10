using Jing;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// Resources资源的管理器
    /// </summary>
    public class ResourcesResMgr : AResMgr
    {
        public ResourcesResMgr()
        {
            
        }

        public override string[] GetDepends(string abName)
        {
            return new string[0];
        }

        public override T Load<T>(string abName, string assetName)
        {
            string path = AssetBundlePath2ResourcePath(abName, assetName);
            var asset = Resources.Load<T>(path);
            return asset;
        }

        public override void LoadAsync(string abName, string assetName, Action<UnityEngine.Object> onLoaded, Action<float> onProgress = null)
        {
            ILBridge.Ins.StartCoroutine(ResourceLoadAsync(AssetBundlePath2ResourcePath(abName, assetName), onLoaded, onProgress));
        }

        public override void Unload(string abName, bool isUnloadAllLoaded = false, bool isUnloadDepends = true)
        {
            Resources.UnloadUnusedAssets();
        }

        public override void UnloadAll(bool isUnloadAllLoaded = false)
        {
            Resources.UnloadUnusedAssets();
        }

        IEnumerator ResourceLoadAsync(string assetPath, Action<UnityEngine.Object> onLoaded, Action<float> onProgress)
        {
            ResourceRequest rr = Resources.LoadAsync(assetPath);
            do
            {
                if (onProgress != null)
                {
                    onProgress.Invoke(rr.progress);
                }
                yield return new WaitForEndOfFrame();
            }
            while (false == rr.isDone);

            //加载完成
            onLoaded.Invoke(rr.asset);
        }

        /// <summary>
        /// 将AB资源的路径格式转换为Resources目录中资源的路径
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        string AssetBundlePath2ResourcePath(string abName, string assetName)
        {
            abName = ABNameWithoutExtension(abName);
            if (abName.ToLower() != ZeroConst.ROOT_AB_FILE_NAME) //resources表示从根目录获取资源，则不需要添加目录
            {
                assetName = FileSystem.CombinePaths(abName, assetName);
            }
            return assetName;
        }
    }
}
