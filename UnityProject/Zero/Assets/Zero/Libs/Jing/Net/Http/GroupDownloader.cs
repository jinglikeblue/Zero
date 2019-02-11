using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Jing
{
    /// <summary>
    /// 资源下载队列
    /// </summary>
    public class GroupDownloader
    {
        /// <summary>
        /// 加载信息
        /// </summary>
        public struct LoadInfo
        {
            /// <summary>
            /// 加载对象URL
            /// </summary>
            public string url;

            /// <summary>
            /// 保存位置
            /// </summary>
            public string savePath;

            /// <summary>
            /// 文件版本号
            /// </summary>
            public string version;

            /// <summary>
            /// 加载完成的回调
            /// </summary>
            public Action<object> onLoaded;

            /// <summary>
            /// 加载完成回调携带的数据
            /// </summary>
            public object data;

            /// <summary>
            /// 加载文件的大小(bytes)
            /// </summary>
            public long fileSize;

            public LoadInfo(string url, string savePath, string version, long fileSize, Action<object> onLoaded, object data)
            {
                this.url = url;
                this.savePath = savePath;
                this.version = version;
                this.fileSize = fileSize;
                this.onLoaded = onLoaded;
                this.data = data;
            }
        }

        long _totalSize = 0;
        /// <summary>
        /// 下载文件总大小
        /// </summary>
        public long TotalSize
        {
            get { return _totalSize; }
        }

        long _loadedSize = 0;

        float _progress;
        /// <summary>
        /// 下载进度
        /// </summary>
        public float Progress
        {
            get { return _progress; }
        }

        string _error;
        public string Error
        {
            get { return _error; }
        }

        bool _isDone;
        public bool IsDone
        {
            get
            {
                lock (_loadedQueue)
                {
                    while (_loadedQueue.Count > 0)
                    {
                        LoadInfo info = _loadedQueue.Dequeue();
                        info.onLoaded.Invoke(info.data);
                    }
                }
                return _isDone;
            }
        }

        public int Count
        {
            get { return _infoList.Count; }
        }

        List<LoadInfo> _infoList = new List<LoadInfo>();
        int _idx;
        bool _isLoadding = false;
        Queue<LoadInfo> _loadedQueue = new Queue<LoadInfo>();

        public void AddLoad(string url, string savePath, string version, long fileSize = 1, Action<object> onLoaded = null, object data = null)
        {
            if (_isLoadding)
            {
                return;
            }            
            _infoList.Add(new LoadInfo(url, savePath, version, fileSize,  onLoaded, data));
            _totalSize += fileSize;
        }

        public void StartLoad()
        {
            if (_isLoadding)
            {
                return;
            }
            _loadedSize = 0;
            new Thread(LoadThread).Start();
        }

        void LoadThread()
        {
            _isLoadding = true;

            _progress = 0;
            _idx = 0;
            
            while (_idx < _infoList.Count)
            {
                LoadInfo info = _infoList[_idx];
                Downloader loader = new Downloader(info.url, info.savePath, info.version);
                do
                {
                    double loaderLoaded = info.fileSize * loader.progress;
                    var tempLoadedSize = _loadedSize + loaderLoaded;
                    _progress = (float)(tempLoadedSize / _totalSize); 
                    //Debug.LogFormat("下载进度  idx:{0} , progress:{1}[{2}/{3}]", _idx, _progress, tempLoadedSize, _totalSize);
                    Thread.Sleep(20);
                }
                while (false == loader.isDone);                                               

                if (loader.error != null)
                {
                    _error = string.Format("[{0}] {1}", info.url, loader.error);
                    break;
                }

                if (info.onLoaded != null)
                {
                    lock (_loadedQueue)
                    {
                        _loadedQueue.Enqueue(info);
                    }
                }
                _loadedSize += info.fileSize;
                _idx++;
            }

            _progress = 1;
            _loadedSize = _totalSize;
            _isDone = true;
            _isLoadding = false;
        }
    }
}