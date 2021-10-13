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
            _localPath = FileUtility.CombinePaths(Runtime.Ins.localResDir, "setting.json");
            
            if (Runtime.Ins.IsLoadAssetsFromNet && Runtime.Ins.localData.IsUpdateSetting)
            {
                ILBridge.Ins.StartCoroutine(Update());
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

        IEnumerator Update()
        {
            var list = Runtime.Ins.SettingFileNetDirList;
            for(var i = 0; i < list.Length; i++)
            {
                var settingFileUrl = FileUtility.CombinePaths(list[i], "setting.json");
                Debug.Log(Log.Zero1("开始下载setting.json: {0}", settingFileUrl));
                Downloader loader = new Downloader(settingFileUrl, _localPath, DateTime.UtcNow.ToFileTimeUtc().ToString());
                while (false == loader.isDone)
                {
                    yield return new WaitForEndOfFrame();
                }
                loader.Dispose();
                if (null == loader.error)
                {
                    SettingVO vo = LoadLocalSetting();
                    Runtime.Ins.setting = vo;
                    Runtime.Ins.netResDir = FileUtility.CombineDirs(true, vo.netResRoot, ZeroConst.PLATFORM_DIR_NAME);

                    _onLoaded();
                    yield break;
                }
                else
                {
                    Debug.Log(Log.Zero2("setting.json下载失败: {0}", settingFileUrl));
                }
            }

            var errorMsg = "所有指向setting.json的URL都不可使用！";
            Debug.LogErrorFormat(errorMsg);
            _onError.Invoke(errorMsg);
        }
    }
}
