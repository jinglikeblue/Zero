using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using UnityEngine;

namespace Jing
{
    /// <summary>
    /// 资源下载器
    /// </summary>
    public class Downloader
    {
        /// <summary>
        /// 下载连接数限制
        /// PS:修改该值可以直接简单的限制HTTP下载请求的并发数
        /// </summary>
        public static int downloadConnectionLimit = 500;

        /// <summary>
        /// 重写的WebClient类
        /// </summary>
        class DownloadWebClient : WebClient
        {
            readonly int _timeout;
            public DownloadWebClient(int timeout = 60)
            {
                _timeout = timeout * 1000;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {                
                HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.Timeout = _timeout;
                request.ReadWriteTimeout = _timeout;
                request.Proxy = null;
                return request;
            }

            protected override WebResponse GetWebResponse(WebRequest request)
            {
                return base.GetWebResponse(request);
            }
        }

        DownloadWebClient _client;

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
            get { return _client == null ? true : false; }
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
            _client = new DownloadWebClient();            
            _client.DownloadProgressChanged += OnDownloadProgressChanged;
            _client.DownloadFileCompleted += OnDownloadFileCompleted;

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
                _client.DownloadFileAsync(uri, savePath);                                
            }
            catch (Exception ex)
            {
                _isDone = true;
                _error = ex.Message;
            }
        }

        /// <summary>
        /// 销毁对象，会停止所有的下载
        /// </summary>
        public void Dispose()
        {
            if (_client != null)
            {
                _client.DownloadProgressChanged -= OnDownloadProgressChanged;
                _client.DownloadFileCompleted -= OnDownloadFileCompleted;
                _client.CancelAsync();
                _client.Dispose();
                _client = null;
                if(false ==_isDone)
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
                SetError("Disconnected");                
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
            if(e.BytesReceived > _loadedSize)
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
            if(ts.TotalMilliseconds > timeout)
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
            if(File.Exists(savePath))
            {
                File.Delete(savePath);
            }
            _error = error;
        }
    }
}
