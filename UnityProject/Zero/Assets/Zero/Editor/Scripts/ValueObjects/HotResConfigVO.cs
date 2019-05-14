using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero.Edit
{
    /// <summary>
    /// 热更资源配置
    /// </summary>
    public class HotResConfigVO
    {
        /// <summary>
        /// 发布目录
        /// </summary>
        public string resDir = "./res";

        /// <summary>
        /// 是否发布后打开发布目录
        /// </summary>
        public bool isOpenDirOnPublished = true;

        #region AssetBundle配置

        /// <summary>
        /// 是否保留「.manifest」文件
        /// </summary>
        public bool isKeepManifest = false;

        /// <summary>
        /// 热更资源目录，该目录为Assets下的某一个子目录
        /// </summary>
        public string abHotResDir = "";

        #endregion

        #region DLL配置

        /// <summary>
        /// 热更脚本目录
        /// </summary>
        public string ilScriptDir = "";

        /// <summary>
        /// 热更脚本项目目录
        /// </summary>
        public string ilProjDir = "";

        /// <summary>
        /// Visual Studio编译工具地址
        /// </summary>
        public string devenvPath;

        /// <summary>
        /// 热更脚本项目csproj路径
        /// </summary>
        public string ilProjCsprojPath;

        #endregion

        #region 资源版本号配置

        #endregion
    }
}