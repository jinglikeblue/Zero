using Jing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    /// <summary>
    /// Zero编辑器的常量
    /// </summary>
    public class ZeroEditorConst
    {
        /// <summary>
        /// 项目目录
        /// </summary>
        static public string PROJECT_PATH = Directory.GetParent(Application.dataPath).FullName;

        /// <summary>
        /// 编辑器配置文件目录
        /// </summary>
        static public string EDITOR_CONFIG_DIR = FileSystem.CombineDirs(false, ZeroConst.ZERO_LIBRARY_DIR, "EditorConfigs");

        /// <summary>
        /// DLL打包的发布目录
        /// </summary>
        static public string DLL_PUBLISH_DIR = FileSystem.CombineDirs(false, ZeroConst.PUBLISH_RES_ROOT_DIR, ZeroConst.DLL_DIR_NAME);

        /// <summary>
        /// AssetBundle打包的发布目录
        /// </summary>
        static public string ASSET_BUNDLE_PUBLISH_DIR = FileSystem.CombineDirs(false, ZeroConst.PUBLISH_RES_ROOT_DIR, ZeroConst.AB_DIR_NAME);

        /// <summary>
        /// 配置的发布目录
        /// </summary>
        static public string CONFIG_PUBLISH_DIR = FileSystem.CombineDirs(false, ZeroConst.PUBLISH_RES_ROOT_DIR, ZeroConst.CONFIG_DIR_NAME);

        /// <summary>
        /// AssetBundle打包缓存目录
        /// </summary>
        static public string ASSET_BUNDLE_CACHE_DIR = FileSystem.CombineDirs(false, ZeroConst.ZERO_LIBRARY_DIR, "ReleaseCache", ZeroConst.AB_DIR_NAME);

        /// <summary>
        /// 热更代码的根目录
        /// </summary>
        static public string HOT_SCRIPT_ROOT_DIR = "Assets/@Scripts";

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
