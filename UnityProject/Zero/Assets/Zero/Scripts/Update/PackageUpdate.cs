using Jing;
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
        public void Start(Action onComplete, Action<float, long> onProgress)
        {
            Log.CI(Log.COLOR_BLUE, "「PackageUpdate」内嵌资源解压检查...");
            ILBridge.Ins.StartCoroutine(Run(onComplete, onProgress));
        }

        IEnumerator Run(Action onComplete, Action<float, long> onProgress)
        {
            do
            {
                //检查程序是否第一次启动
                if (Runtime.Ins.localData.IsInit)
                {
                    break;
                }

                Runtime.Ins.localData.IsInit = true;

                Debug.Log("PackageZip:" + ZeroConst.STREAMING_ASSETS_PATH);
                Debug.Log("PackageZip:" + ZeroConst.PACKAGE_ZIP_FILE_NAME);

                //检查是否存在Package.zip
                string path = FileSystem.CombinePaths(ZeroConst.STREAMING_ASSETS_PATH, ZeroConst.PACKAGE_ZIP_FILE_NAME);
                WWW www = new WWW(path);
                while (false == www.isDone)
                {
                    onProgress(0f, 0);
                    yield return new WaitForEndOfFrame();
                }

                //Package.zip不存在
                if (null != www.error)
                {
                    Log.I("解压[{0}]:{1}", ZeroConst.PACKAGE_ZIP_FILE_NAME, www.error);
                    break;
                }

                //解压Zip
                ZipHelper zh = new ZipHelper();
                zh.UnZip(www.bytes, Runtime.Ins.localResDir);
                while (false == zh.isDone)
                {
                    Log.I("[{0}]解压进度:{1}%", ZeroConst.PACKAGE_ZIP_FILE_NAME, zh.progress * 100);
                    onProgress(zh.progress, www.bytes.Length);
                    yield return new WaitForEndOfFrame();
                }
                www.Dispose();

                Log.I("[{0}]解压完成", ZeroConst.PACKAGE_ZIP_FILE_NAME);

                //重新加载一次版本号文件，因为可能被覆盖了
                Runtime.Ins.localResVer.Load();
            } while (false);
            onComplete();
            yield break;
        }
    }
}
