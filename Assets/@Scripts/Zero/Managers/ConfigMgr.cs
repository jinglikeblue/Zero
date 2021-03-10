using Jing;
using LitJson;
using System;
using System.IO;
using System.Text;
using Zero;

namespace ZeroHot
{
    /// <summary>
    /// 配置管理工具
    /// </summary>
    class ConfigMgr : ASingleton<ConfigMgr>
    {        
        CacheMap<string, object> _cache = new CacheMap<string, object>();

        /// <summary>
        /// 加载JSON配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T LoadJsonConfig<T>(string path, bool cacheEnable = true)
        {
            string json = LoadTextConfig(path, cacheEnable);
            var vo = JsonMapper.ToObject<T>(json);
            return vo;
        }

        /// <summary>
        /// 加载文本配置
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string LoadTextConfig(string path, bool cacheEnable = true)
        {
            path = FileUtility.CombinePaths(ConfigsRootDir, path);

            string text = null;

            if (cacheEnable && _cache.Contines(path))
            {                
                var success = _cache.Get(path, out text);
                if (success)
                {
                    return text;
                }
            }

            if (!File.Exists(path))
            {
                //配置不存在
                throw new Exception(string.Format("[{0}] 文件不存在", path));
            }

            text = File.ReadAllText(path, Encoding.UTF8);
            if (cacheEnable)
            {
                _cache.Set(path, text);
            }
            return text;
        }

        /// <summary>
        /// 加载二进制数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public byte[] LoadBytesConfig(string path, bool cacheEnable = true)
        {
            path = FileUtility.CombinePaths(ConfigsRootDir, path);

            byte[] bytes = null;

            if (cacheEnable && _cache.Contines(path))
            {
                if(_cache.Get(path, out bytes));                
                {
                    return bytes;
                }
            }

            if (!File.Exists(path))
            {
                //配置不存在
                throw new Exception(string.Format("[{0}] 文件不存在", path));
            }

            bytes = File.ReadAllBytes(path);
            if (cacheEnable)
            {
                _cache.Set(path, bytes);
            }

            return bytes;
        }

        /// <summary>
        /// 加载自动化工具的配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadZeroHotConfig<T>(bool cacheEnable = true)
        {
            var type = typeof(T);

            var atts = type.GetCustomAttributes(typeof(ZeroHotConfigAttribute), false);
            var att = atts[0] as ZeroHotConfigAttribute;

            if (null == att)
            {
                //加载的配置不存在
                throw new Exception(string.Format("[{0}] 并不是一个配置数据对象", type.FullName));
            }

            return LoadJsonConfig<T>(att.path, cacheEnable);
        }

        /// <summary>
        /// 配置根目录
        /// </summary>
        string ConfigsRootDir
        {
            get
            {
                string path = null;
                switch (Runtime.Ins.VO.hotResMode)
                {
                    case EHotResMode.NET_ASSET_BUNDLE:
                        path = FileUtility.CombineDirs(false, ZeroConst.WWW_RES_PERSISTENT_DATA_PATH, ZeroConst.CONFIG_DIR_NAME);
                        break;
                    case EHotResMode.LOCAL_ASSET_BUNDLE:
                        path = FileUtility.CombineDirs(false, ZeroConst.PUBLISH_RES_ROOT_DIR, ZeroConst.CONFIG_DIR_NAME);
                        break;
                    case EHotResMode.ASSET_DATA_BASE:
                        //该种开发模式下，直接从Asset/@Configs取文件
                        path = FileUtility.CombineDirs(false, ZeroConst.HOT_CONFIGS_ROOT_DIR);
                        break;
                }
                return path;
            }
        }

        public override void Destroy()
        {
            _cache.Clear();
        }

        protected override void Init()
        {
            
        }
    }
}
