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

                string packageZipFilePath = FileUtility.CombinePaths(Application.streamingAssetsPath, ZeroConst.PACKAGE_ZIP_FILE_NAME);

                if (Application.platform == RuntimePlatform.Android)
                {
                    //如果是Android真机环境下，需要用android原生代码将文件拷贝到可读写目录，再进行操作
                    string androidPackageZipTempPath = FileUtility.CombinePaths(ZeroConst.PERSISTENT_DATA_PATH, ZeroConst.PACKAGE_ZIP_FILE_NAME);

                    Debug.LogFormat("压缩文件：{0}", packageZipFilePath);
                    Debug.LogFormat("临时压缩文件：{0}", androidPackageZipTempPath);

                    //请求Android原生代码，用文件流形式复制文件到临时位置
                    AndroidJavaObject javaAssetFileCopy = new AndroidJavaObject("pieces.jing.zerolib.file.AssetFileCopy");                    
                    var isRequestSuccess = javaAssetFileCopy.Call<bool>("copyAssetsFile", "package.zip", androidPackageZipTempPath);
                    if (isRequestSuccess)
                    {
                        while (false == javaAssetFileCopy.Call<bool>("isDone"))
                        {
                            yield return new WaitForEndOfFrame();
                        }

                        var error = javaAssetFileCopy.Call<string>("error");
                        if (error != null)
                        {
                            Debug.LogFormat("copyAssetsFile出现问题：" + error);
                        }
                        else
                        {
                            packageZipFilePath = androidPackageZipTempPath;
                        }
                    }
                }

                //检查是否存在Package.zip
                if (!File.Exists(packageZipFilePath))
                {
                    Debug.LogFormat("解压的文件[{0}]不存在", packageZipFilePath);
                    break;
                }

                Debug.LogFormat("压缩文件：{0}", packageZipFilePath);
                Debug.LogFormat("解压目录：{0}", ZeroConst.PERSISTENT_DATA_PATH);

                //解压Zip
                ZipHelper zh = new ZipHelper();
                //将文件解压到可读写目录中
                zh.UnZip(packageZipFilePath, ZeroConst.PERSISTENT_DATA_PATH);

                while (false == zh.isDone)
                {
                    Debug.LogFormat("[{0}]解压进度:{1}% [{2}/{3}]", ZeroConst.PACKAGE_ZIP_FILE_NAME, zh.progress * 100, zh.decompessSize, zh.totalSize);
                    onProgress(zh.progress, zh.totalSize);
                    yield return new WaitForEndOfFrame();
                }

                if (zh.error != null)
                {
                    Debug.LogFormat("解压出错：\n{0}", zh.error);
                }
                else
                {
                    Debug.LogFormat("[{0}]解压进度:{1}% [{2}/{3}]", ZeroConst.PACKAGE_ZIP_FILE_NAME, zh.progress * 100, zh.decompessSize, zh.totalSize);
                    Debug.LogFormat("[{0}]解压完成", ZeroConst.PACKAGE_ZIP_FILE_NAME);
                }

                //重新加载一次版本号文件，因为可能被覆盖了
                Runtime.Ins.localResVer.Load();
            } while (false);
            onComplete();
            yield break;
        }
    }
}
