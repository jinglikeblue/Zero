using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero.Edit
{
    public class IOSProjectInitConfig 
    {
        const string CONFIG_NAME = "IOSProjectInit.json";

        public class ConfigVO
        {
            public string[] frameworkToProjectList = new string[0];

            public Dictionary<string,string> file2BuildList = new Dictionary<string, string>();

            public Dictionary<string, string> buildPropertyList = new Dictionary<string, string>();

            public Dictionary<string, string> pListDataList = new Dictionary<string, string>();

            public string[] urlSchemeList = new string[0];

            public string[] appQueriesSchemeList = new string[0];
        }

        public ConfigVO Cfg { get; private set; }

        public IOSProjectInitConfig()
        {
            Cfg = EditorConfigUtil.LoadConfig<ConfigVO>(CONFIG_NAME);
            if (null == Cfg)
            {
                Cfg = new ConfigVO();
                SaveCfg();
            }
        }

        public void SaveCfg()
        {
            EditorConfigUtil.SaveConfig(Cfg, CONFIG_NAME);
        }
        
    }
}