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
        /// AssetBundle文件存储的后缀名
        /// </summary>
        public const string AB_EXTENSION = ".ab";

        /// <summary>
        /// 存储AssetBundle之间依赖关系的manifest文件
        /// </summary>
        public const string MANIFEST_FILE_NAME = "manifest";

        /// <summary>
        /// 直接放在Resources目录下的资源，会被打包到resources.ab文件中
        /// </summary>
        public const string SPECIAL_AB_NAME = "resources";


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
                    string _streamingAssetsPath = Application.streamingAssetsPath;
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