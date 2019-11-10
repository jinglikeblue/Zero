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
        Action<string> _onError;

        string _localPath;

        public void Start(Action onLoaded, Action<string> onError)
        {
            Debug.Log(Log.Zero1("「SettingUpdate」配置文件更新检查..."));
            _onLoaded = onLoaded;
            _onError = onError;
            _localPath = FileSystem.CombinePaths(Runtime.Ins.localResDir, "setting.json");
            
            if (Runtime.Ins.IsLoadAssetsFromNet && Runtime.Ins.localData.IsUpdateSetting)
            {
                var netPath = FileSystem.CombinePaths(Runtime.Ins.netResDir , "setting.json");
                Debug.Log(Log.Zero1("配置文件: {0}", netPath));
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
                Debug.LogErrorFormat(loader.error);
                if (null != _onError)
                {
                    _onError.Invoke(loader.error);
                }
                yield break;
            }
            loader.Dispose();

            SettingVO vo = LoadLocalSetting();
            Runtime.Ins.setting = vo;
            Runtime.Ins.netResDir = FileSystem.CombineDirs(true, vo.netResRoot, ZeroConst.PLATFORM_DIR_NAME);

            _onLoaded();
            yield break;
        }
    }
}
