using Jing;
using System;
using System.Collections;
using System.IO;
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
            Debug.Log(Log.Zero1("「PackageUpdate」内嵌资源解压检查..."));
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
                    Debug.LogFormat("解压[{0}]:{1}", ZeroConst.PACKAGE_ZIP_FILE_NAME, www.error);
                    break;
                }

                //先将这个文件写到可读写目录中
                var zipFilePath = FileSystem.CombinePaths(ZeroConst.PERSISTENT_DATA_PATH, "package.zip");
                File.WriteAllBytes(zipFilePath, www.bytes);

                //解压Zip
                ZipHelper zh = new ZipHelper();
                //将文件解压到可读写目录中
                zh.UnZip(zipFilePath, ZeroConst.PERSISTENT_DATA_PATH);

                while (false == zh.isDone)
                {
                    Debug.LogFormat("[{0}]解压进度:{1}%", ZeroConst.PACKAGE_ZIP_FILE_NAME, zh.progress * 100);
                    onProgress(zh.progress, www.bytes.Length);
                    yield return new WaitForEndOfFrame();
                }
                www.Dispose();

                Debug.LogFormat("[{0}]解压完成", ZeroConst.PACKAGE_ZIP_FILE_NAME);

                //重新加载一次版本号文件，因为可能被覆盖了
                Runtime.Ins.localResVer.Load();
            } while (false);
            onComplete();
            yield break;
        }
    }
}
