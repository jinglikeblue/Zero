using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero.Edit
{
    public class HotResPublishModel
    {
        const string CONFIG_NAME = "HotResCfg.json";
        HotResConfigVO _cfgVO;

        public HotResConfigVO CfgVO
        {
            get
            {
                return _cfgVO;
            }
        }

        public HotResPublishModel()
        {
            LoadConfig();
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        public void LoadConfig()
        {
            _cfgVO = EditorConfigUtil.LoadConfig<HotResConfigVO>(CONFIG_NAME);
            if (null == _cfgVO)
            {
                _cfgVO = new HotResConfigVO();
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public void SaveConfig()
        {
            EditorConfigUtil.SaveConfig(_cfgVO, CONFIG_NAME);
        }

        /// <summary>
        /// 构建热更AssetBundle资源
        /// </summary>
        public void BuildAssetBundle()
        {

        }


        /// <summary>
        /// 构建热更DLL文件
        /// </summary>
        public void BuildDll()
        {

        }

        /// <summary>
        /// 构建res.json文件
        /// </summary>
        public void BuildResJsonFile()
        {

        }
    }
}