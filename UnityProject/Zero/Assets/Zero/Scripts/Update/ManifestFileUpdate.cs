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
        Action<string> _onError;

        string _localPath;

        Runtime _rt;

        string _manifestName;

        public void Start(Action onUpdate, Action<string> onError)
        {
            Log.CI(Log.COLOR_BLUE, "「ManifestFileUpdate」Manifest描述文件更新检查...");
            _rt = Runtime.Ins;
            _manifestName = FileSystem.CombinePaths(ZeroConst.AB_DIR_NAME, ZeroConst.MANIFEST_FILE_NAME + ZeroConst.AB_EXTENSION);
            _onUpdate = onUpdate;
            _onError = onError;
            _localPath = FileSystem.CombinePaths(_rt.localResDir , _manifestName);

            if (Runtime.Ins.IsLoadAssetsFromNet && false == _rt.netResVer.IsSameVer(_manifestName, _rt.localResVer))
            {
                ILBridge.Ins.StartCoroutine(Update(_rt.netResDir + _manifestName, _rt.netResVer.GetVer(_manifestName)));
            }
            else
            {
                InitResMgr();
            }
        }

        void InitResMgr()
        {
            if(Runtime.Ins.IsHotResProject)
            {
                if (Runtime.Ins.IsLoadAssetsByAssetDataBase)
                {
                    ResMgr.Ins.Init(ResMgr.EResMgrType.ASSET_DATA_BASE, Runtime.Ins.VO.hotResRoot);
                }
                else
                {
                    ResMgr.Ins.Init(ResMgr.EResMgrType.ASSET_BUNDLE, _localPath);
                }
            }
            else
            {
                ResMgr.Ins.Init(ResMgr.EResMgrType.RESOURCES, _localPath);
            }

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
                if (null != _onError)
                {
                    _onError.Invoke(loader.error);
                }
                yield break;
            }
            loader.Dispose();

            _rt.localResVer.SetVerAndSave(_manifestName, ver);

            InitResMgr();
            yield break;
        }

    }
}
