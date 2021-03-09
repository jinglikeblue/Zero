using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Jing
{
    /// <summary>
    /// 资源下载器
    /// </summary>
    public class Downloader
    {
        class HttpDownloader
        {
            /// <summary>
            /// 下载取消的错误
            /// </summary>
            public const string ERROR_CANCEL = "ERROR_CANCEL";

            /// <summary>
            /// 下载缓冲区大小
            /// </summary>
            public const int BUFFER_SIZE = 65536;

            /// <summary>
            /// 下载过程中
            /// </summary>
            public event Action<HttpDownloader, long, long> onProgress;

            /// <summary>
            /// 下载完成
            /// </summary>
            public event Action<HttpDownloader, string> onComplete;

            /// <summary>
            /// 下载的Url
            /// </summary>
            public string Url { get; private set; }

            /// <summary>
            /// 保存地址
            /// </summary>
            public string SavePath { get; private set; }

            /// <summary>
            /// 已下载大小
            /// </summary>
            public long LoadedSize { get; private set; } = 0;

            /// <summary>
            /// 总大小
            /// </summary>
            public long TotalSize { get; private set; } = 0;

            /// <summary>
            /// 错误信息
            /// </summary>
            public string Error { get; private set; } = null;

            /// <summary>
            /// 是否正在下载中
            /// </summary>
            public bool IsDownloading
            {
                get
                {
                    return _thread != null ? true : false;
                }
            }

            Thread _thread;

            public HttpDownloader(string url, string savePath)
            {
                Url = url;
                SavePath = savePath;
            }

            public void Download()
            {
                if (null == _thread)
                {
                    _thread = new Thread(new ThreadStart(DownloadProcess));
                    _thread.Start();
                }
                else
                {
                    throw new Exception("Is Downloading");
                }
            }

            public void Cancel()
            {
                onProgress = null;
                onComplete = null;
                _thread = null;
            }

            void DownloadProcess()
            {
                try
                {
                    // 设置参数
                    HttpWebRequest request = WebRequest.Create(Url) as HttpWebRequest;

                    //发送请求并获取相应回应数据
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                    LoadedSize = 0;
                    TotalSize = response.ContentLength;

                    //直到request.GetResponse()程序才开始向目标网页发送Post请求
                    Stream rs = response.GetResponseStream();

                    //创建本地文件写入流
                    FileStream fs = new FileStream(SavePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                    byte[] buffer = new byte[BUFFER_SIZE];

                    while (_thread != null)
                    {
                        int size = rs.Read(buffer, 0, buffer.Length);
                        if (size > 0)
                        {
                            LoadedSize += size;
                            fs.Write(buffer, 0, size);
                            onProgress?.Invoke(this, LoadedSize, TotalSize);
                            Thread.Sleep(1);
                        }
                        else
                        {
                            break;
                        }
                    }

                    fs.Close();
                    rs.Close();

                    if (null == _thread)
                    {
                        Error = ERROR_CANCEL;                                               
                    }
                    else
                    {
                        Error = null;
                        _thread = null;                       
                    }                    
                }
                catch (Exception e)
                {
                    Error = e.Message;                    
                }
                finally
                {
                    onComplete?.Invoke(this, Error);
                }
            }
        }

        /// <summary>
        /// 下载连接数限制
        /// PS:修改该值可以直接简单的限制HTTP下载请求的并发数
        /// </summary>
        public static int downloadConnectionLimit = 500;

        HttpDownloader _httpDownloader;

        bool _isDone;

        /// <summary>
        /// 是否操作完成
        /// </summary>
        public bool isDone
        {
            get
            {                
                if (false == _isDone)
                {
                    CheckTimeout();
                }
                return _isDone;
            }
        }

        float _progress;

        /// <summary>
        /// 操作进度
        /// </summary>
        public float progress
        {
            get
            {
                return _progress;
            }
        }

        string _error;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string error
        {
            get
            {
                return _error;
            }
        }

        long _totalSize;

        /// <summary>
        /// 文件总大小
        /// </summary>
        public long totalSize
        {
            get
            {
                return _totalSize;
            }
        }

        long _loadedSize;

        /// <summary>
        /// 已完成大小
        /// </summary>
        public long loadedSize
        {
            get
            {
                return _loadedSize;
            }
        }

        /// <summary>
        /// 是否已销毁
        /// </summary>
        public bool isDisposeed
        {
            get { return _httpDownloader == null ? true : false; }
        }


        string _savePath;

        string _url;

        /// <summary>
        /// 下载的URL地址
        /// </summary>
        public string url
        {
            get { return _url; }
        }

        /// <summary>
        /// 文件的保存路径
        /// </summary>
        public string savePath
        {
            get { return _savePath; }
        }

        /// <summary>
        /// 下载超时的设置，当指定毫秒内下载进度没有改变时，视为下载超时。
        /// </summary>
        public int timeout = 15000;

        /// <summary>
        /// 最后进度改变的时间
        /// </summary>
        DateTime _lastProgressChangedDT;

        /// <summary>
        /// 初始化下载类
        /// </summary>
        /// <param name="url">下载文件的URL地址</param>
        /// <param name="savePath">保存文件的本地地址</param>
        /// <param name="version">URL对应文件的版本号</param>
        public Downloader(string url, string savePath, string version = null)
        {
            _url = url;
            _savePath = savePath;
            string saveDir = Path.GetDirectoryName(savePath);
            if (Directory.Exists(saveDir) == false)
            {
                Directory.CreateDirectory(saveDir);
            }

            if (null != version)
            {
                string flag;
                if (url.Contains("?"))
                {
                    flag = "&";
                }
                else
                {
                    flag = "?";
                }

                url += string.Format("{0}unity_download_ver={1}", flag, version);
            }

            try
            {
                Uri uri = new Uri(url);
                var serverPoint = ServicePointManager.FindServicePoint(uri);
                serverPoint.ConnectionLimit = downloadConnectionLimit;
                _progress = 0;
                _lastProgressChangedDT = DateTime.Now;
                _httpDownloader = new HttpDownloader(url, savePath);
                _httpDownloader.onProgress += OnDownloadProgress;
                _httpDownloader.onComplete += OnDownloadComplete;
                _httpDownloader.Download();
            }
            catch (Exception ex)
            {
                _isDone = true;
                _error = ex.Message;
            }
        }

        private void OnDownloadComplete(HttpDownloader loader, string error)
        {
            if (error != null)
            {
                SetError(error);
            }
            else if (_loadedSize < _totalSize)
            {
                SetError($"文件大小不一致: {_loadedSize} / {_totalSize}");
            }
            _isDone = true;
        }

        private void OnDownloadProgress(HttpDownloader loader, long loaded, long total)
        {
            _lastProgressChangedDT = DateTime.Now;
            _loadedSize = loader.LoadedSize;
            _totalSize = total;
            if (0 == _totalSize)
            {
                _progress = 0;
            }
            else
            {
                _progress = _loadedSize / (float)_totalSize;
            }
        }

        /// <summary>
        /// 销毁对象，会停止所有的下载
        /// </summary>
        public void Dispose()
        {
            if (_httpDownloader != null)
            {
                _httpDownloader.onProgress -= OnDownloadProgress;
                _httpDownloader.onComplete -= OnDownloadComplete;
                _httpDownloader.Cancel();
                _httpDownloader = null;
                if (false == _isDone)
                {
                    SetError("Canceled");
                    _isDone = true;
                }
            }
        }

        /// <summary>
        /// 下载文件完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                SetError(e.Error.Message);
            }
            else if (_loadedSize < _totalSize)
            {
                SetError($"文件大小不一致: {_loadedSize} / {_totalSize}");
            }
            _isDone = true;
        }

        /// <summary>
        /// 下载进度改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (e.BytesReceived > _loadedSize)
            {
                _lastProgressChangedDT = DateTime.Now;
                _loadedSize = e.BytesReceived;
                _totalSize = e.TotalBytesToReceive;
                if (0 == _totalSize)
                {
                    _progress = 0;
                }
                else
                {
                    _progress = _loadedSize / (float)_totalSize;
                }
            }
        }

        /// <summary>
        /// 检查是否超时
        /// </summary>
        void CheckTimeout()
        {
            TimeSpan ts = DateTime.Now - _lastProgressChangedDT;
            //Debug.LogFormat("检查时间差：{0} {1}", ts.TotalMilliseconds, url);
            if (ts.TotalMilliseconds > timeout)
            {
                //超时
                Dispose();
                SetError("TimeOut");
            }
        }

        void SetError(string error)
        {
            Debug.LogErrorFormat("下载失败 [{0}] ：{1}", _url, error);

            //删除文件
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
            _error = error;
        }
    }
}
