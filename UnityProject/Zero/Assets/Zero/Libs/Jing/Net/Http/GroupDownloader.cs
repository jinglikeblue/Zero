using System;
using System.Collections.Generic;
using System.Threading;

namespace Jing
{
    /// <summary>
    /// 资源下载队列
    /// </summary>
    public class GroupDownloader
    {
        public struct LoadInfo
        {
            public string url;
            public string savePath;
            public string version;
            public Action<object> onLoaded;
            public object data;

            public LoadInfo(string url, string savePath, string version, Action<object> onLoaded = null, object data = null)
            {
                this.url = url;
                this.savePath = savePath;
                this.version = version;
                this.onLoaded = onLoaded;
                this.data = data;
            }
        }


        float _progress;
        public float progress
        {
            get { return _progress; }
        }

        string _error;
        public string error
        {
            get { return _error; }
        }

        bool _isDone;
        public bool isDone
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


        public void AddLoad(string url, string savePath, int version, Action<object> onLoaded = null, object data = null)
        {
            AddLoad(url, savePath, version.ToString(), onLoaded, data);
        }

        public void AddLoad(string url, string savePath, string version, Action<object> onLoaded = null, object data = null)
        {
            if (_isLoadding)
            {
                return;
            }            
            _infoList.Add(new LoadInfo(url, savePath, version, onLoaded, data));
        }

        public void StartLoad()
        {
            if (_isLoadding)
            {
                return;
            }
            new Thread(LoadThread).Start();
        }

        void LoadThread()
        {
            _isLoadding = true;

            _progress = 0;
            _idx = 0;

            float progressPiece = 1f / _infoList.Count;
            while (_idx < _infoList.Count)
            {
                LoadInfo info = _infoList[_idx];
                Downloader loader = new Downloader(info.url, info.savePath, info.version);
                while (false == loader.isDone)
                {
                    _progress = ((float)_idx * progressPiece) + (loader.progress * progressPiece);
                    //Debug.LogFormat("下载进度  idx:{0} , progressPiece:{1} , loader.progress:{2} , progress:{3}", _idx, progressPiece, loader.progress, _progress);
                    Thread.Sleep(20);
                }

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

                _idx++;
            }

            _progress = 1;
            _isDone = true;
            _isLoadding = false;
        }
    }
}