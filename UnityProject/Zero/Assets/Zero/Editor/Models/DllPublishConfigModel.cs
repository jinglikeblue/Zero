using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero.Edit
{
    public class DllPublishConfigModel
    {
        const string CONFIG_NAME = "DllCfg.json";

        DllPublishConfigVO _cfg;       

        public DllPublishConfigVO VO
        {
            get
            {
                return _cfg;
            }
        }

        public DllPublishConfigModel()
        {
            _cfg = EditorConfigUtil.LoadConfig<DllPublishConfigVO>(CONFIG_NAME);
            if(null == _cfg)
            {
                _cfg = new DllPublishConfigVO();
            }
        }

        public void Save()
        {
            EditorConfigUtil.SaveConfig(_cfg, CONFIG_NAME);
        }
    }
}