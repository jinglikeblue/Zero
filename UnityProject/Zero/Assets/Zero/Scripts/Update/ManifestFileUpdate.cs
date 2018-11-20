using Jing;
using System;
using System.Collections;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// AssetBundle资源的Manifest描述文件，该文件很重要，描述了AssetBundle资源之间的依赖关系
    /// </summary>
    public class ManifestFileUpdate
    {
        Action _onUpdate;

        string _localPath;

        Runtime _rt;

        string _manifestName;

        public void Start(Action onUpdate)
        {
            Log.CI(Log.COLOR_BLUE, "「ManifestFileUpdate」Manifest描述文件更新检查...");
            _rt = Runtime.Ins;
            _manifestName = _rt.netResVer.VO.manifestName;
            _onUpdate = onUpdate;
            _localPath = _rt.localResDir + _manifestName;

            if (Runtime.Ins.IsLoadFromNet && false == _rt.netResVer.IsSameVer(_manifestName, _rt.localResVer))
            {
                CoroutineBridge.Ins.Run(Update(_rt.netResDir + _manifestName, _rt.netResVer.GetVer(_manifestName)));
            }
            else
            {
                InitAssetBundleMgr();
            }
        }

        void InitAssetBundleMgr()
        {
            ResMgr.Ins.Init(Runtime.Ins.IsLoadABFromResources ? ResMgr.EResMgrType.RESOURCES : ResMgr.EResMgrType.ASSET_BUNDLE, _localPath);
            _onUpdate();
        }

        IEnumerator Update(string url, string ver)
        {
            Downloader loader = new Downloader(url, _localPath, ver);
            while (false == loader.isDone)
            {
                yield return new WaitForEndOfFrame();
            }

            if (null != loader.error)
            {
                Log.E(loader.error);
                yield break;
            }
            loader.Dispose();

            _rt.localResVer.SetVerAndSave(_manifestName, ver);

            InitAssetBundleMgr();
            yield break;
        }

    }
}
