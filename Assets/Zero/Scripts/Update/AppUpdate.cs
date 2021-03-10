using Jing;
using System;
using System.Collections;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 客户端更新基类
    /// </summary>
    public class AppUpdate
    {
        const string APK_INSTALL_FILE_EXT = ".apk";

        Action<bool> _onOver;
        protected Action<float, long> _onProgress;
        protected Action<string> _onError;

        /// <summary>
        /// 开始检查更新
        /// </summary>
        /// <param name="onOver">不用更新的回调</param>
        public void Start(Action<bool> onOver, Action<float, long> onProgress, Action<string> onError)
        {
            Debug.Log(Log.Zero1("「{0}」客户端版本号检查...", this.GetType().Name));
            if (false == Runtime.Ins.IsLoadAssetsFromNet)
            {
                onOver(false);
                return;
            }

            _onOver = onOver;
            _onProgress = onProgress;
            _onError = onError;
            int result = CheckVersionCode(Application.version, Runtime.Ins.setting.client.version);

            Debug.Log(Log.Zero1("客户端版本号 本地: {0}   网络: {1}  更新地址：{2}", Application.version, Runtime.Ins.setting.client.version, Runtime.Ins.setting.client.url));

            if (result == -1)
            {
                Uri updateURI = null;
                try
                {
                    updateURI = new Uri(Runtime.Ins.setting.client.url);
                    var url = string.IsNullOrEmpty(updateURI.Query) ? $"{updateURI.OriginalString}?ver={Runtime.Ins.setting.client.version}" : updateURI.AbsoluteUri;

                    if (updateURI.AbsolutePath.EndsWith(APK_INSTALL_FILE_EXT))
                    {
                        //是APK安装文件
                        ILBridge.Ins.StartCoroutine(UpdateAPK(url));
                    }
                    else
                    {                        
                        Application.OpenURL(url);
                    }
                }
                catch
                {
                    _onError?.Invoke("更新App失败！");                        
                }                
            }
            else
            {
                _onOver.Invoke(result == 1 ? true : false);
            }            
        }

        IEnumerator UpdateAPK(string apkUrl)
        {
            Downloader loader = new Downloader(apkUrl, FileUtility.CombinePaths(Runtime.Ins.localResDir, ZeroConst.ANDROID_APK_NAME));

            Debug.Log($"安装包保存路径:{loader.savePath}");
            
            while (!loader.isDone)
            {
                _onProgress?.Invoke(loader.progress, loader.totalSize);
                yield return new WaitForEndOfFrame();
            }

            _onProgress?.Invoke(1, loader.totalSize);

            if (loader.error != null)
            {
                _onError?.Invoke(loader.error);
                yield break;
            }

#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                var apkInstallUtilityCls = new AndroidJavaClass("pieces.jing.zerolib.utilities.ApkInstallUtility");
                var installResult = apkInstallUtilityCls.CallStatic<bool>("install", loader.savePath);
                if (false == installResult)
                {
                    Debug.LogError("拉起安装程序失败");
                }
                else
                {
                    Debug.Log("拉起安装程序");
                }
            }
            else
            {
                Debug.Log("真机环境下，会拉起安装Apk！");
            }
#endif
        }



        /// <summary>
        /// 检查版本编码，如果本地号大于网络，则返回1，等于返回0，小于返回-1
        /// </summary>
        /// <param name="local"></param>
        /// <param name="net"></param>
        /// <returns></returns>
        protected int CheckVersionCode(string local, string net)
        {
            string[] locals = local.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            string[] nets = net.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            int compareLength = locals.Length > nets.Length ? locals.Length : nets.Length;

            for (int i = 0; i < compareLength; i++)
            {
                int lc = 0;
                if (i < locals.Length)
                {
                    lc = int.Parse(locals[i]);
                }

                int nc = 0;
                if (i < nets.Length)
                {
                    nc = int.Parse(nets[i]);
                }

                if (lc > nc)
                {
                    return 1;
                }
                else if (lc < nc)
                {
                    return -1;
                }
            }

            return 0;
        }
    }
}