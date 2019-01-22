using Jing;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Zero
{
    public class PCUpdate : AClientUpdate
    {
        public PCUpdate()
        {
            CheckRoot();
        }
        public override void OnNeedUpdate()
        {
            ILBridge.Ins.StartCoroutine(UpdateEXE());
        }

        IEnumerator UpdateEXE()
        {
            //下载APK
            string savePath = GetUpdateFilePath("update.exe");
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
        /// <summary>
        /// 给桌面快捷方式图标授予“以管理员身份执行”权限
        /// </summary>
        void CheckRoot()
        {
            try
            {
                string lnkName = string.Format("//{0}.lnk", Application.productName);
                using (FileStream fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + lnkName, FileMode.Open, FileAccess.ReadWrite))
                {

                    fs.Seek(21, SeekOrigin.Begin);

                    fs.WriteByte(0x22);
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.Log("修改快捷方式失败");
            }
        }

        /// <summary>
        /// 更新文件本地路径(桌面)   
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        string GetUpdateFilePath(string fileName)
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string path = dir + "/" + fileName;
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            return path;
        }
    }
}
