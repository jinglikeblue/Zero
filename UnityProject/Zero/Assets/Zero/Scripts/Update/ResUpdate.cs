using Jing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 资源更新
    /// </summary>
    public class ResUpdate
    {
        Action _onComplete;
        Action<float> _onProgress;
        string[] _groups;

        public void Start(string name, Action onComplete, Action<float> onProgress = null)
        {
            Start(new string[] { name }, onComplete, onProgress);
        }

        public void Start(string[] groups, Action onComplete, Action<float> onProgress = null)
        {            
            _onComplete = onComplete;
            _onProgress = onProgress;
            _groups = groups;

            StartLoadResVerFile();
        }

        void StartLoadResVerFile()
        {
            new ResVerFileUpdate().Start(StartUpdateManifest);
        }

        void StartUpdateManifest()
        {
            new ManifestFileUpdate().Start(StartLoadRes);
        }

        void StartLoadRes()
        {
            Log.CI(Log.COLOR_BLUE, "「ResUpdate」资源文件更新...");
            if (false == Runtime.Ins.IsLoadFromNet)
            {
                _onComplete();
                return;
            }

            //整理出所有需要资源的清单（包括依赖的）
            HashSet<string> itemSet = new HashSet<string>();
            for(int i = 0; i < _groups.Length; i++)
            {
                string group = _groups[i];
                var itemList = GetItemsInGroup(group);
                foreach(var itemName in itemList)
                {
                    itemSet.Add(itemName);
                }
            }            

            CoroutineBridge.Ins.StartCoroutine(UpdateGroups(itemSet));
        }

        List<string> GetItemsInGroup(string group)
        {
            List<string> nameList = new List<string>();
            var itemList = Runtime.Ins.netResVer.FindGroup(group);
            foreach (var item in itemList)
            {
                nameList.Add(item.name);                
                nameList.AddRange(GetAllDepends(item.name));
            }
            return nameList;
        }

        List<string> GetAllDepends(string itemName)
        {
            List<string> nameList = new List<string>();
            string abDir = ResMgr.Ins.RootDir.Replace(Runtime.Ins.localResDir,"");
            abDir += "/";
            if (false == itemName.StartsWith(abDir))
            {
                return nameList;
            }

            
            string abName = itemName.Replace(abDir, "");
            var abDependList = ResMgr.Ins.GetDepends(abName);
            foreach(var ab in abDependList)
            {
                nameList.Add(FileSystem.CombinePaths(abDir, ab));
            }

            return nameList;
        }

        IEnumerator UpdateGroups(HashSet<string> itemSet)
        {
            //实例化一个资源组下载器
            GroupDownloader groupLoader = new GroupDownloader();
            foreach (var itemName in itemSet)
            {
                string localVer = Runtime.Ins.localResVer.GetVer(itemName);
                var netItem = Runtime.Ins.netResVer.Get(itemName);

                if (localVer != netItem.version)
                {
                    //将要下载的文件依次添加入下载器
                    groupLoader.AddLoad(Runtime.Ins.netResDir + itemName, Runtime.Ins.localResDir + itemName, netItem.version, OnItemLoaded, netItem);
                }
            }
            //启动下载器开始下载
            groupLoader.StartLoad();

            //判断是否所有资源下载完成，如果没有，返回一个下载的进度（该进度表示的整体进度）
            do
            {
                _onProgress.Invoke(groupLoader.progress);
                yield return new WaitForEndOfFrame();
            }
            while (false == groupLoader.isDone);

            //判断下载是否返回错误
            if (null != groupLoader.error)
            {
                Log.E("下载出错：{0}", groupLoader.error);
                yield break;
            }            

            _onComplete();
        }

        private void OnItemLoaded(object obj)
        {
            var item = (ResVerVO.Item)obj;
            Runtime.Ins.localResVer.SetVerAndSave(item.name, item.version);
        }
    }
}