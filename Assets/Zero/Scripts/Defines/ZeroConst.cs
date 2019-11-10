using Jing;
using System;
using System.IO;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// Zero框架的常量
    /// </summary>
    public class ZeroConst
    {
        /// <summary>
        /// 热更AssetBundle资源的目录名称
        /// </summary>
        public const string AB_DIR_NAME = "ab";

        /// <summary>
        /// 热更DLL资源的目录名称
        /// </summary>
        public const string DLL_DIR_NAME = "dll";

        /// <summary>
        /// @Scripts中的代码启动类
        /// </summary>
        public const string LOGIC_SCRIPT_STARTUP_CLASS_NAME = "ZeroHot.Main";

        /// <summary>
        /// @Scripts中的代码启动方法
        /// </summary>
        public const string LOGIC_SCRIPT_STARTUP_METHOD = "Startup";

        /// <summary>
        /// 配置资源的目录名称
        /// </summary>
        public const string CONFIG_DIR_NAME = "configs";

        /// <summary>
        /// 热更DLL的文件名称（不含后缀）
        /// </summary>
        public const string DLL_FILE_NAME = "scripts";

        /// <summary>
        /// 资源版本描述文件的名称
        /// </summary>
        public const string RES_JSON_FILE_NAME = "res.json";

        /// <summary>
        /// 启动配置文件名称
        /// </summary>
        public const string SETTING_FILE_NAME = "setting.json";

        /// <summary>
        /// 预置文件压缩包名称
        /// </summary>
        public const string PACKAGE_ZIP_FILE_NAME = "package.zip";

        /// <summary>
        /// AssetBundle文件存储的后缀名
        /// </summary>
        public const string AB_EXTENSION = ".ab";

        /// <summary>
        /// 存储AssetBundle之间依赖关系的manifest文件
        /// </summary>
        public const string MANIFEST_FILE_NAME = "manifest";

        /// <summary>
        /// 直接放在Assets/@Resources目录下的资源，会被打包到root_assets.ab文件中
        /// </summary>
        public const string ROOT_AB_FILE_NAME = "root_assets";

        #region 基于项目根目录的路径

        /// <summary>
        /// 热更资源在项目中的根目录
        /// </summary>
        static public string HOT_RESOURCES_ROOT_DIR = "Assets/@Resources";

        /// <summary>
        /// 热更配置在项目中的根目录
        /// </summary>
        static public string HOT_CONFIGS_ROOT_DIR = "Assets/@Configs";

        /// <summary>
        /// Zero框架的Library目录
        /// </summary>
        static public string ZERO_LIBRARY_DIR = "LibraryZero";

        /// <summary>
        /// 热更资源发布目录
        /// </summary>
        static public string PUBLISH_RES_ROOT_DIR = FileSystem.CombineDirs(false, ZERO_LIBRARY_DIR, "Release", "res", PLATFORM_DIR_NAME);

        #endregion

        static string _platformDirName = null;

        /// <summary>
        /// 平台目录
        /// </summary>
        public static string PLATFORM_DIR_NAME
        {
            get
            {
                if (null == _platformDirName)
                {
#if UNITY_STANDALONE
                    _platformDirName = "pc";
#elif UNITY_IPHONE
        _platformDirName = "ios";
#elif UNITY_ANDROID
                _platformDirName = "android";
#endif
                }

                return _platformDirName;
            }
        }

        static string _streamingAssetsPath = null;

        /// <summary>
        /// 可用WWW加载资源的streamingAssets目录地址
        /// </summary>
        public static string STREAMING_ASSETS_PATH
        {
            get
            {
                if (null == _streamingAssetsPath)
                {
                    _streamingAssetsPath = Application.streamingAssetsPath;
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IPHONE
                    //如果在编辑器下，或是PC平台或iOS平台，则要加上file://才能读取资源
                    _streamingAssetsPath = "file://" + _streamingAssetsPath;
#endif
                }
                return _streamingAssetsPath;
            }
        }

        static string _persistentDataPath = null;

        /// <summary>
        /// 可读写目录地址
        /// </summary>
        public static string PERSISTENT_DATA_PATH
        {
            get
            {
                if (null == _persistentDataPath)
                {
                    _persistentDataPath = Application.persistentDataPath;
#if UNITY_EDITOR
                    _persistentDataPath = FileSystem.CombineDirs(false, Directory.GetParent(Application.dataPath).FullName, ZERO_LIBRARY_DIR, "RuntimeCaches");
#elif UNITY_STANDALONE
                _persistentDataPath = FileSystem.CombineDirs(false, Application.dataPath, "Caches");
#endif
                }
                return _persistentDataPath;
            }
        }

        /// <summary>
        /// 网络下载的更新资源存储的目录
        /// </summary>
        public static string WWW_RES_PERSISTENT_DATA_PATH = FileSystem.CombineDirs(false, PERSISTENT_DATA_PATH, "zero", "res");

        /// <summary>
        /// 框架生成文件存放地址
        /// </summary>
        public static string GENERATES_PERSISTENT_DATA_PATH = FileSystem.CombineDirs(false, PERSISTENT_DATA_PATH, "zero", "generated");
    }
}