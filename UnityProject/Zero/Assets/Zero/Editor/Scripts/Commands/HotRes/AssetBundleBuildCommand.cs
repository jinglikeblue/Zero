using Jing;
using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace Zero.Edit
{
    class AssetBundleBuildCommand
    {
        const string MARK = "zero";

        string _resDir;
        string _abResourcesDir;
        bool _isKeepManifest;

        public AssetBundleBuildCommand(string resDir, string abResourcesDir, bool isKeepManifest)
        {
            _resDir = resDir;
            _abResourcesDir = abResourcesDir;
            _isKeepManifest = isKeepManifest;
        }

        public void Execute()
        {           
            //标记资源
            MarkABRes();
            //打包
            Build();
        }

        void Build()
        {
            var releaseDir = FileSystem.CombineDirs(false, _resDir, ZeroEditorUtil.PlatformDirName, HotResConst.AB_DIR_NAME);
            if (Directory.Exists(releaseDir))
            {
                //删除目标目录
                Directory.Delete(releaseDir, true);
            }
            new AssetBundleBuilder(releaseDir, "Assets/Resources").Start();

            if (false == _isKeepManifest)
            {
                FileSystem.DeleteFilesByExt(releaseDir, ".manifest");
            }
        }

        /// <summary>
        /// 标记目标目录的资源
        /// </summary>
        void MarkABRes()
        {
            string[] allABNames = AssetDatabase.GetAllAssetBundleNames();
            foreach (var abName in allABNames)
            {
                AssetDatabase.RemoveAssetBundleName(abName, true);
            }                                 
            
            UnityEngine.Debug.LogFormat("标记资源：{0}", _abResourcesDir);
            MarkRes2ABUtil.Mark(_abResourcesDir, MARK, "");
        }
    }    
}