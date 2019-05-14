using Jing;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zero;

namespace Zero
{
    /// <summary>
    /// 检查指定的资源是否需要更新，如果需要，会返回需要更新的资源的列表。
    /// </summary>
    public class ResUpdateChecker
    {
        /// <summary>
        /// 是否需要更新资源的版本文件至最新版本（默认为true）
        /// </summary>
        public bool needUpdateResVerFile = true;

        /// <summary>
        /// 是否需要更新资源的Manifest文件至最新版本（默认为true）
        /// </summary>
        public bool needUpdateManifestFile = true;

        string[] _groups;

        Action<string[]> _onComplete;
        Action<string> _onError;

        public ResUpdateChecker(bool needUpdateResVerFile = true, bool needUpdateManifestFile = true)
        {
            this.needUpdateResVerFile = needUpdateResVerFile;
            this.needUpdateManifestFile = needUpdateManifestFile;
        }

        public void Start(string name, Action<string[]> onComplete, Action<string> onError)
        {           
            Start(new string[] { name }, onComplete, onError);
        }

        public void Start(string[] groups, Action<string[]> onComplete, Action<string> onError)
        {
            _onError = onError;
            _onComplete = onComplete;
            _groups = groups;
            StartLoadResVerFile();
        }

        void StartLoadResVerFile()
        {
            if (needUpdateResVerFile)
            {
                new ResVerFileUpdate().Start(StartUpdateManifest, _onError);
            }
            else
            {
                StartUpdateManifest();
            }
        }

        void StartUpdateManifest()
        {
            if (needUpdateResVerFile)
            {
                new ManifestFileUpdate().Start(StartCheckRes, _onError);
            }
            else
            {
                StartCheckRes();
            }
        }

        void StartCheckRes()
        {
            if (false == Runtime.Ins.IsLoadAssetsFromNet)
            {
                _onComplete(new string[0]);
                return;
            }

            //整理出所有需要资源的清单（包括依赖的）
            HashSet<string> itemSet = new HashSet<string>();
            for (int i = 0; i < _groups.Length; i++)
            {
                string group = _groups[i];
                var itemList = GetItemsInGroup(group);
                foreach (var itemName in itemList)
                {
                    itemSet.Add(itemName);
                }
            }

            List<string> needUpdateList = new List<string>();
            //开始检查版本，找出需要更新的资源
            foreach (var itemName in itemSet)
            {
                string localVer = Runtime.Ins.localResVer.GetVer(itemName);
                var netItem = Runtime.Ins.netResVer.Get(itemName);

                if (localVer != netItem.version)
                {
                    //版本一致，不需要更新
                    needUpdateList.Add(itemName);
                }
            }

            _onComplete(needUpdateList.ToArray());
        }

        List<string> GetItemsInGroup(string group)
        {
            List<string> nameList = new List<string>();
            var itemList = Runtime.Ins.netResVer.FindGroup(group);
            foreach (var item in itemList)
            {
                nameList.Add(item.name);
                var depends = GetAllDepends(item.name);
                nameList.AddRange(depends);

                Log.CI(Log.COLOR_PURPLE, "版本检查的资源：{0}", item.name);
                if (depends.Count > 0)
                {
                    Log.CI(Log.COLOR_PURPLE, "依赖的资源列表:");
                    foreach (var depend in depends)
                    {
                        Log.CI(Log.COLOR_PURPLE, "              {0}", depend);
                    }
                }
            }
            return nameList;
        }

        /// <summary>
        /// 得到目标所有的依赖
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        List<string> GetAllDepends(string itemName)
        {
            List<string> nameList = new List<string>();
            string abDir = ResMgr.Ins.RootDir.Replace(Runtime.Ins.localResDir, "");
            abDir += "/";
            if (false == itemName.StartsWith(abDir))
            {
                return nameList;
            }


            string abName = itemName.Replace(abDir, "");
            var abDependList = ResMgr.Ins.GetDepends(abName);
            foreach (var ab in abDependList)
            {
                nameList.Add(FileSystem.CombinePaths(abDir, ab));
            }

            return nameList;
        }
    }
}