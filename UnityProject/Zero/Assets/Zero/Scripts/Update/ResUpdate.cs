using Jing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 资源更新（只需要指定需要下载的资源，依赖的资源会自动添加到下载队列）
    /// </summary>
    public class ResUpdate
    {
        /// <summary>
        /// 是否需要更新资源的版本文件至最新版本（默认为true）
        /// </summary>
        public bool needUpdateResVerFile = true;

        /// <summary>
        /// 是否需要更新资源的Manifest文件至最新版本（默认为true）
        /// </summary>
        public bool needUpdateManifestFile = true;

        public ResUpdate(bool needUpdateResVerFile = true, bool needUpdateManifestFile = true)
        {
            this.needUpdateResVerFile = needUpdateResVerFile;
            this.needUpdateManifestFile = needUpdateManifestFile;
        }

        Action _onComplete;
        Action<float, long> _onProgress;
        Action<string> _onError;
        string[] _groups;

        public void Start(string name, Action onComplete, Action<float, long> onProgress = null, Action<string> onError = null)
        {
            Start(new string[] { name }, onComplete, onProgress);
        }

        public void Start(string[] groups, Action onComplete, Action<float, long> onProgress = null, Action<string> onError = null)
        {
            _onComplete = onComplete;
            _onProgress = onProgress;
            _onError = onError;
            _groups = groups;

            StartResUpdateCheck();
        }

        void StartResUpdateCheck()
        {
            new ResUpdateChecker(needUpdateResVerFile, needUpdateManifestFile).Start(_groups, OnResUpdateChecked, _onError);
        }

        private void OnResUpdateChecked(string[] needUpdateResList)
        {
            ILBridge.Ins.StartCoroutine(UpdateGroups(needUpdateResList));
        }

        IEnumerator UpdateGroups(string[] needUpdateResList)
        {
            //实例化一个资源组下载器
            GroupDownloader groupLoader = new GroupDownloader();
            foreach (var resName in needUpdateResList)
            {
                var netItem = Runtime.Ins.netResVer.Get(resName);

                //将要下载的文件依次添加入下载器
                groupLoader.AddLoad(FileSystem.CombinePaths(Runtime.Ins.netResDir, resName), FileSystem.CombinePaths(Runtime.Ins.localResDir, resName), netItem.version, netItem.size, OnItemLoaded, netItem);
            }
            //启动下载器开始下载
            groupLoader.StartLoad();

            //判断是否所有资源下载完成，如果没有，返回一个下载的进度（该进度表示的整体进度）
            do
            {
                _onProgress.Invoke(groupLoader.Progress, groupLoader.TotalSize);
                yield return new WaitForEndOfFrame();
            }
            while (false == groupLoader.IsDone);

            //判断下载是否返回错误
            if (null != groupLoader.Error)
            {
                if (null != _onError)
                {
                    _onError.Invoke(groupLoader.Error);
                }
                yield break;
            }

            _onComplete();
        }

        private void OnItemLoaded(object obj)
        {
            var item = (ResVerVO.Item)obj;
            Log.CI(Log.COLOR_BLUE, "下载完成：{0} Ver:{1}", item.name, item.version);
            Runtime.Ins.localResVer.SetVerAndSave(item.name, item.version);
        }
    }
}