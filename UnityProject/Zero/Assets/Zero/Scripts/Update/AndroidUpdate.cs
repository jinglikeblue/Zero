using Jing;
using System.Collections;
using UnityEngine;

namespace Zero
{
    public class AndroidUpdate : AClientUpdate
    {
        /// <summary>
        /// 执行更新
        /// </summary>
        public override void OnNeedUpdate()
        {
            ILBridge.Ins.StartCoroutine(UpdateAPK());
        }

        IEnumerator UpdateAPK()
        {
            //下载APK
            string savePath = string.Format("{0}update.apk", Runtime.Ins.localResDir);
            Downloader loader = new Downloader(Runtime.Ins.setting.client.url, savePath, Runtime.Ins.setting.client.version);
            while(false == loader.isDone)
            {
                _onProgress.Invoke(loader.progress, loader.totalSize);
                yield return new WaitForEndOfFrame();
            }

            if(loader.error != null)
            {
                Log.E(loader.error);
                if (null != _onError)
                {
                    _onError.Invoke(loader.error);
                }
                yield break;
            }


            /*
             * 启动APK安装程序(调用Android原生代码,这里使用了zero.aar原生代码库）
             * 仅支持Target API Level不超过 Android 6.0 (API Level 23)的项目。
            */
            AndroidJavaObject ajo = new AndroidJavaObject("pieces.jing.zero.ZeroLib");            
            bool success = ajo.Call<bool>("install", savePath);
            if (success)
            {
                //开始安装
            }
            else
            {
                Log.E("无法安装，打开网页");
                Application.OpenURL(Runtime.Ins.setting.client.url);
            }
            yield break;
        }

    }
}
