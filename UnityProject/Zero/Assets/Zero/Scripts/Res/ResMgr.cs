using System;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    public class ResMgr
    {
        public enum EResMgrType
        {
            ASSET_BUNDLE,
            RESOURCES,
        }

        static ResMgr _ins = new ResMgr();

        public static ResMgr Ins
        {
            get { return _ins; }
        }

        private ResMgr()
        {

        }

        AResMgr _mgr;

        /// <summary>
        /// 资源根目录
        /// </summary>
        public string RootDir
        {
            get { return _mgr.RootDir; }
        }

        public void Init(EResMgrType type, string manifestFilePath)
        {
            switch (type)
            {
                case EResMgrType.ASSET_BUNDLE:
                    Log.CI(Log.COLOR_BLUE, "初始化资源管理器... 资源来源：[AssetBundle]  Manifest路径：{0}", manifestFilePath);
                    var newMgr = new AssetBundleResMgr(manifestFilePath);
                    if (_mgr != null && _mgr is AssetBundleResMgr)
                    {
                        //替换旧的需要继承一下已加载字典
                        newMgr.Inherit(_mgr as AssetBundleResMgr);
                    }
                    _mgr = newMgr;
                    break;
                case EResMgrType.RESOURCES:
                    Log.CI(Log.COLOR_BLUE, "初始化资源管理器... 资源来源：[Resources]");
                    _mgr = new ResourcesResMgr(manifestFilePath);                    
                    break;
            }
        }

        /// <summary>
        /// 执行一次内存回收(该接口开销大，可能引起卡顿)
        /// </summary>
        public void DoGC()
        {
            //移除没有引用的资源
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        /// <summary>
        /// 得到AB资源的依赖
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public string[] GetDepends(string abName)
        {
            return _mgr.GetDepends(abName);
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="abName">资源包名称</param>
        /// <param name="isUnloadAllLoaded">是否卸载Hierarchy中的资源</param>
        /// <param name="isUnloadDepends">是否卸载关联的资源</param>
        public void Unload(string abName, bool isUnloadAllLoaded = false, bool isUnloadDepends = true)
        {
            _mgr.Unload(abName, isUnloadAllLoaded, isUnloadDepends);
        }

        /// <summary>
        /// 卸载所有资源
        /// </summary>
        /// <param name="isUnloadAllLoaded">是否卸载Hierarchy中的资源</param>
        public void UnloadAll(bool isUnloadAllLoaded = false)
        {
            _mgr.UnloadAll(isUnloadAllLoaded);
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName">资源包名称</param>
        /// <param name="assetName">资源名称</param>
        /// <returns></returns>
        public T Load<T>(string abName, string assetName) where T : UnityEngine.Object
        {
            return _mgr.Load<T>(abName, assetName);
        }

        /// <summary>
        /// 异步加载一个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName">资源包名称</param>
        /// <param name="assetName">资源名称</param>
        /// <param name="onLoaded"></param>
        /// <param name="onProgress"></param>
        public void LoadAsync(string abName, string assetName, Action<UnityEngine.Object> onLoaded, Action<float> onProgress = null)
        {
            _mgr.LoadAsync(abName, assetName, onLoaded, onProgress);
        }
    }
}
