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

        string GetNameWithoutExt(string name)
        {
            var ext = Path.GetExtension(name);
            if (!string.IsNullOrEmpty(ext))
            {
                name = name.Replace(ext, "");
            }
            return name;
        }

        public override string[] GetDepends(string abName)
        {
            return new string[0];
        }

        public override UnityEngine.Object Load(string abName, string assetName)
        {
            string path = AssetBundlePath2ResourcePath(abName, assetName);
            var asset = Resources.Load(path);
            return asset;
        }

        public override T Load<T>(string abName, string assetName)
        {           
            string path = AssetBundlePath2ResourcePath(abName, assetName);            
            var asset = Resources.Load<T>(path);
            return asset;
        }

        public override void LoadAsync(string abName, string assetName, Action<UnityEngine.Object> onLoaded, Action<float> onProgress = null)
        {           
            ILBridge.Ins.StartCoroutine(ResourceLoadAsync<UnityEngine.Object>(AssetBundlePath2ResourcePath(abName, assetName), onLoaded, onProgress));
        }

        public override void LoadAsync<T>(string abName, string assetName, Action<T> onLoaded, Action<float> onProgress = null)
        {            
            ILBridge.Ins.StartCoroutine(ResourceLoadAsync<T>(AssetBundlePath2ResourcePath(abName, assetName), onLoaded, onProgress));
        }

        public override void Unload(string abName, bool isUnloadAllLoaded = false, bool isUnloadDepends = true)
        {
            Resources.UnloadUnusedAssets();
        }

        public override void UnloadAll(bool isUnloadAllLoaded = false)
        {
            Resources.UnloadUnusedAssets();
        }

        IEnumerator ResourceLoadAsync<T>(string assetPath, Action<T> onLoaded, Action<float> onProgress) where T : UnityEngine.Object
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
            onLoaded.Invoke((T)rr.asset);
        }

        /// <summary>
        /// 将AB资源的路径格式转换为Resources目录中资源的路径
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        string AssetBundlePath2ResourcePath(string abName, string assetName)
        {
            assetName = GetNameWithoutExt(assetName);

            abName = ABNameWithoutExtension(abName);
            if (abName.ToLower() != ZeroConst.ROOT_AB_FILE_NAME) //resources表示从根目录获取资源，则不需要添加目录
            {
                assetName = FileSystem.CombinePaths(abName, assetName);
            }
            return assetName;
        }
    }
}
