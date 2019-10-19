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

        /// <summary>
        /// 热更资源在项目中根目录
        /// </summary>
        static public string HOT_RESOURCES_ROOT_DIR = "Assets/@Resources";


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
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID
                    //如果在编辑器下，或是PC平台或Android平台，则要加上file://才能读取资源
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
                    _persistentDataPath = FileSystem.CombineDirs(false, Directory.GetParent(Application.dataPath).FullName, "Caches");
#elif UNITY_STANDALONE
                _persistentDataPath = FileSystem.CombineDirs(false, Application.dataPath, "Caches");
#endif
                }
                return _persistentDataPath;
            }
        }
    }
}