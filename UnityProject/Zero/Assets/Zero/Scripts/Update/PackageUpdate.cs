using System;
using System.Collections;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 内嵌资源包解压
    /// </summary>
    public class PackageUpdate
    {
        public void Start(Action onComplete, Action<float> onProgress)
        {
            Log.CI(Log.COLOR_BLUE, "「PackageUpdate」内嵌资源解压检查...");
            if (false == Runtime.Ins.IsLoadFromNet)
            {
                onComplete();
                return;
            }
            CoroutineBridge.Ins.StartCoroutine(Run(onComplete, onProgress));
        }

        IEnumerator Run(Action onComplete, Action<float> onProgress)
        {
            do
            {
                //检查程序是否第一次启动
                if (Runtime.Ins.localData.IsInit)
                {
                    break;
                }

                Runtime.Ins.localData.IsInit = true;

                //检查是否存在Package.zip
                string path = Runtime.Ins.streamingAssetsPath + "Package.zip";
                WWW www = new WWW(path);
                while (false == www.isDone)
                {
                    onProgress(www.progress * 0.5f);
                    yield return new WaitForEndOfFrame();
                }

                //Package.zip不存在
                if (null != www.error)
                {
                    break;
                }

                //解压Zip
                ZipHelper zh = new ZipHelper();
                zh.UnZip(www.bytes, Runtime.Ins.localResDir);
                while (false == zh.isDone)
                {
                    onProgress((zh.progress * 0.5f) + 0.5f);
                    yield return new WaitForEndOfFrame();
                }
                www.Dispose();

            } while (false);
            onComplete();
            yield break;
        }
    }
}
