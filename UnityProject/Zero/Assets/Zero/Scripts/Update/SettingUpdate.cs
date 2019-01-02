using Jing;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 配置文件更新
    /// </summary>
    public class SettingUpdate
    {
        Action _onLoaded;

        string _localPath;

        public void Start(Action onLoaded)
        {
            Log.CI(Log.COLOR_BLUE,"「SettingUpdate」配置文件更新检查...");
            _onLoaded = onLoaded;
            _localPath = Runtime.Ins.localResDir + "setting.json";
            
            if (Runtime.Ins.IsLoadFromNet)
            {
                var netPath = Runtime.Ins.netResDir + "setting.json";
                Log.CI(Log.COLOR_BLUE, "配置文件: {0}", netPath);
                ILBridge.Ins.StartCoroutine(Update(netPath));
            }
            else
            {                
                Runtime.Ins.setting = LoadLocalSetting();
                _onLoaded();
            }
        }

        SettingVO LoadLocalSetting()
        {
            string settingJsonStr = File.ReadAllText(_localPath);
            SettingVO vo = LitJson.JsonMapper.ToObject<SettingVO>(settingJsonStr);
            return vo;
        }

        IEnumerator Update(string url)
        {
            Downloader loader = new Downloader(url, _localPath, DateTime.UtcNow.ToFileTimeUtc().ToString());            
            while(false == loader.isDone)
            {                
                yield return new WaitForEndOfFrame();
            }

            if (null != loader.error)
            {
                Log.E(loader.error);
                yield break;
            }
            loader.Dispose();

            SettingVO vo = LoadLocalSetting();
            Runtime.Ins.setting = vo;
            Runtime.Ins.netResDir = FileSystem.CombineDirs(true, vo.netResRoot, Runtime.Ins.platform);                  
            
            if (Runtime.Ins.setting.settingJump.ContainsKey(Application.version))
            {
                ILBridge.Ins.StartCoroutine(Update(Runtime.Ins.setting.settingJump[Application.version]));
            }
            else
            {
                _onLoaded();
            }
            yield break;
        }
    }
}
