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
            CoroutineBridge.Ins.StartCoroutine(UpdateAPK());


        }

        IEnumerator UpdateAPK()
        {
            //下载APK
            string savePath = string.Format("{0}update.apk", Runtime.Ins.localResDir);
            Downloader loader = new Downloader(Runtime.Ins.setting.client.url, savePath, Runtime.Ins.setting.client.version);
            while(false == loader.isDone)
            {
                _onProgress.Invoke(loader.progress);
                yield return new WaitForEndOfFrame();
            }

            if(loader.error != null)
            {
                Log.E(loader.error);
                yield break;
            }


            //启动APK安装程序(调用Android原生代码）
            AndroidJavaObject ajo = new AndroidJavaObject("com.jing.gamelibs.GameLib");            
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
