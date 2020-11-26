using Jing;
using System;
using System.Collections.Generic;
using System.Text;
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

        /// <summary>
        /// 检查目标资源组对应的更新列表
        /// <para>注意：该方法并不会更新res.json以及ab.mainifest文件</para>
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        public static string[] CheckUpdateList(string[] groups)
        {
            //整理出所有需要资源的清单（包括依赖的）
            HashSet<string> itemSet = new HashSet<string>();
            for (int i = 0; i < groups.Length; i++)
            {
                string group = groups[i];
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

            return needUpdateList.ToArray();
        }

        void StartCheckRes()
        {
            if (false == Runtime.Ins.IsLoadAssetsFromNet)
            {
                _onComplete(new string[0]);
                return;
            }

            var list = CheckUpdateList(_groups);

            _onComplete(list);
        }

        static List<string> GetItemsInGroup(string group)
        {
            List<string> nameList = new List<string>();
            var itemList = Runtime.Ins.netResVer.FindGroup(group);
           
            foreach (var item in itemList)
            {
                nameList.Add(item.name);
                var depends = GetAllDepends(item.name);
                nameList.AddRange(depends);

                var logSB = new StringBuilder();
                logSB.AppendLine(Log.Zero2("进行版本校验的资源：{0}", item.name));                
                if (depends.Count > 0)
                {
                    logSB.AppendLine(Log.Zero2("                 依赖的资源:"));
                    foreach (var depend in depends)
                    {
                        logSB.AppendLine(Log.Zero2("                 {0}", depend));                        
                    } 
                }
                if (logSB.Length > 0)
                {
                    Debug.Log(logSB.ToString());
                }
            }

            return nameList;
        }

        /// <summary>
        /// 得到目标所有的依赖
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        static List<string> GetAllDepends(string itemName)
        {
            List<string> nameList = new List<string>();
            string abDir = Zero.ZeroConst.AB_DIR_NAME;
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