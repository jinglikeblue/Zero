using Jing;
using System.IO;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 「Zero」框架运行时
    /// </summary>
    public class Runtime
    {
        /// <summary>
        /// 构建单例
        /// </summary>
        public static readonly Runtime Ins = new Runtime();

        RuntimeVO _vo;

        /// <summary>
        /// RuntimeVO数据对象
        /// </summary>
        public RuntimeVO VO
        {
            get { return _vo; }
        }

        /// <summary>
        /// 资源模式
        /// </summary>
        public RuntimeVO.EResMode ResMode
        {
            get { return _vo.resMode; }
        }

        /// <summary>
        /// 本地数据
        /// </summary>
        public LocalDataModel localData;

        /// <summary>
        /// 平台目录
        /// </summary>
        public string platform;

        /// <summary>
        /// StreamingAssets目录访问路径
        /// </summary>
        public string streamingAssetsPath;

        /// <summary>
        /// 配置
        /// </summary>
        public SettingVO setting;

        /// <summary>
        /// 基于运行平台的网络资源目录(使用的网络资源的地址)
        /// </summary>
        public string netResDir;

        /// <summary>
        /// 资源对象版本数据
        /// </summary>
        public ResVerModel netResVer;

        /// <summary>
        /// 基于运行平台的本地资源根路径(存放下载文件的目录)
        /// </summary>
        public string localResDir;

        /// <summary>
        /// 本地的资源版本
        /// </summary>
        public LocalResVerModel localResVer;

        /// <summary>
        /// IL的配置
        /// </summary>
        public RuntimeVO.ILCfgVO ILCfg
        {
            get
            {
                return _vo.ilCfg;
            }
        }

        /// <summary>
        /// 是否允许从Resources加载数据
        /// </summary>
        public bool IsLoadABFromResources
        {
            get
            {
                if (_vo.resMode == RuntimeVO.EResMode.INLINE_RELEASE || 
                    _vo.resMode == RuntimeVO.EResMode.NET_LOCAL_AND_RESOURCES_DEBUG)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 是否是从网络加载资源
        /// </summary>
        public bool IsLoadFromNet
        {
            get
            {
                if (_vo.resMode == RuntimeVO.EResMode.NET_RELEASE)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 是否是内嵌资源项目
        /// </summary>
        public bool IsInlineRelease
        {
            get
            {
                return _vo.resMode == RuntimeVO.EResMode.INLINE_RELEASE ? true : false;
            }
        }

        private Runtime()
        {
            
        }

        public void Init(RuntimeVO vo)
        {
            _vo = vo;
            //日志控制
            Log.isActive = vo.logEnable;            

            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    //Android真机环境
                    platform = "android";
                    streamingAssetsPath = Application.streamingAssetsPath + "/";
                    netResDir = FileSystem.CombineDirs(true, _vo.netRoot, platform);
                    localResDir = FileSystem.CombineDirs(true, Application.persistentDataPath);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    //IOS真机环境
                    platform = "ios";
                    streamingAssetsPath = string.Format("file://{0}/Raw/", Application.dataPath);
                    netResDir = FileSystem.CombineDirs(true, _vo.netRoot, platform);
                    localResDir = FileSystem.CombineDirs(true, Application.persistentDataPath);
                    break;
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.LinuxEditor:
                case RuntimePlatform.OSXEditor:
                    //Editor开发环境

#if UNITY_ANDROID
                        platform = "android";
#elif UNITY_IPHONE
                        platform = "ios";
#else
                        platform = "pc";
#endif

                    streamingAssetsPath = string.Format("file://{0}/StreamingAssets/", Application.dataPath);
                    if (_vo.resMode == RuntimeVO.EResMode.NET_LOCAL_DEBUG || _vo.resMode == RuntimeVO.EResMode.NET_LOCAL_AND_RESOURCES_DEBUG)
                    {
                        netResDir = FileSystem.CombineDirs(true, _vo.developResRoot, platform);
                        localResDir = netResDir;                        
                    }
                    else
                    {
                        netResDir = FileSystem.CombineDirs(true, _vo.netRoot, platform);
                        localResDir = FileSystem.CombineDirs(true, Directory.GetParent(Application.dataPath).FullName, "Caches");
                    }
                    break;
                default:
                    //其它真机环境
                    platform = "pc";
                    streamingAssetsPath = string.Format("file://{0}/StreamingAssets/", Application.dataPath);
                    netResDir = FileSystem.CombineDirs(true, _vo.netRoot, platform);
                    localResDir = FileSystem.CombineDirs(true, Application.dataPath, "StreamingAssets");
                    break;
            }

            //确保本地资源目录存在
            if (false == Directory.Exists(localResDir))
            {
                Directory.CreateDirectory(localResDir);
            }

            localData = new LocalDataModel();
            localResVer = new LocalResVerModel(localData);
        }




    }


}