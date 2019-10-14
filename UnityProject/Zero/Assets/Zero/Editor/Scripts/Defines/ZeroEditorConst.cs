using Jing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    /// <summary>
    /// Zero编辑器的常量
    /// </summary>
    public class ZeroEditorConst
    {        
        /// <summary>
        /// 资源发布目录
        /// </summary>
        static public string PUBLISH_RES_ROOT_DIR = FileSystem.CombineDirs(false, "Res", ZeroConst.PLATFORM_DIR_NAME);

        /// <summary>
        /// AssetBundle打包的发布目录
        /// </summary>
        static public string ASSET_BUNDLE_PUBLISH_DIR = FileSystem.CombineDirs(false, PUBLISH_RES_ROOT_DIR, "ab");

        /// <summary>
        /// AssetBundle打包缓存目录
        /// </summary>
        static public string ASSET_BUNDLE_CACHE_DIR = FileSystem.CombineDirs(false, "Library", "ZeroHotResCache", "ab");

        /// <summary>
        /// 热更资源根目录
        /// </summary>
        static public string RES_ROOT_DIR = "Assets/@Resources";

        /// <summary>
        /// 当前发布平台
        /// </summary>
        static public BuildTarget BUILD_PLATFORM
        {
            get
            {
                BuildTarget platform;
#if UNITY_STANDALONE
                platform = BuildTarget.StandaloneWindows;
#elif UNITY_IPHONE
        platform = BuildTarget.iOS;
#elif UNITY_ANDROID
        platform = BuildTarget.Android;
#endif
                return platform;
            }
        }
    }
}
