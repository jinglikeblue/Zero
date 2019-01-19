using Jing;
using System;
using System.Collections;
using UnityEngine;

namespace Zero
{
    public class PCUpdate : AClientUpdate
    {
        public override void OnNeedUpdate()
        {
            ILBridge.Ins.StartCoroutine(UpdateEXE());
        }

        IEnumerator UpdateEXE()
        {
            //下载APK
            string savePath = string.Format("{0}update.apk", Runtime.Ins.localResDir);
            Downloader loader = new Downloader(Runtime.Ins.setting.client.url, savePath, Runtime.Ins.setting.client.version);
            while (false == loader.isDone)
            {
                _onProgress.Invoke(loader.progress);
                yield return new WaitForEndOfFrame();
            }

            if (loader.error != null)
            {
                Log.E(loader.error);
                _onError?.Invoke(loader.error);
                yield break;
            }

            try
            {
                //启动下载的EXE
                System.Diagnostics.Process.Start(savePath);
                Application.Quit();
            }
            catch (Exception ex)
            {
                Log.E("无法安装:{0}", ex.Message);
            }
            yield break;
        }
    }
}
