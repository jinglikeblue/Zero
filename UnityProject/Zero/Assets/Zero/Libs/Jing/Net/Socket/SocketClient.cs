using System;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using Zero;

namespace Jing
{
    /// <summary>
    /// 连接结果的枚举
    /// </summary>
    public enum ESocketConnectionState
    {
        /// <summary>
        /// 已连接
        /// </summary>
        CONNECTED,

        /// <summary>
        /// 连接断开
        /// </summary>
        DISCONNECTED,
    }    

    /// <summary>
    /// 套接字客户端
    /// </summary>
    public class SocketClient 
    {
        /// <summary>
        /// 读取数据的回调
        /// </summary>
        public Action<byte[]> onReadData;

        /// <summary>
        /// 连接状态改变的回调
        /// </summary>
        public Action<ESocketConnectionState> onConnectionStateChange;

        string _host = "";

        /// <summary>
        /// 连接的地址
        /// </summary>
        public string Host
        {
            get
            {
                return _host;
            }
        }

        int _port = 0;

        /// <summary>
        /// 连接的端口
        /// </summary>
        public int Port
        {
            get
            {
                return _port;
            }
        }

        /// <summary>
        /// 连接线程的睡眠时间
        /// </summary>
        int _sleepTime = 30;

        /// <summary>
        /// 是否已连接服务器
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if(_sct == null)
                {
                    return false;
                }

                return _sct.isConnected;
            }
        }

        SocketConnectionThread _sct;

        public SocketClient()
        {

        }

        /// <summary>
        /// 需要定时调用以同步数据
        /// </summary>
        public void Update()
        {
            if(_sct == null)
            {
                return;
            }

            while(_sct.stateQueue.Count > 0)
            {
                ESocketConnectionState stateMsg = (ESocketConnectionState)_sct.stateQueue.Dequeue();                
                onConnectionStateChange(stateMsg);
            }

            //接收数据
            while(_sct.dataReceivedQueue.Count > 0)
            { 
                byte[] data = _sct.dataReceivedQueue.Dequeue() as byte[];
                onReadData(data);
            }
        }

        /// <summary>
        /// 建立连接
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        SocketConnectionThread CreateConnection()
        {
            Disconnect();
            _sct = new SocketConnectionThread(_host, _port, _sleepTime);
            return _sct;
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="onConnectionState"></param>
        public void Connect(string host, int port, int fps = 20)
        {
            _host = host;
            _port = port;
            _sleepTime = 1000 / fps;
            Reconnect();
        }

        /// <summary>
        /// 重连服务器
        /// </summary>
        public void Reconnect()
        {
            CreateConnection();            
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            if (_sct != null)
            {
                _sct.Dispose();
            }
        }

        /// <summary>
        /// 发送二进制数据
        /// </summary>
        /// <param name="data"></param>
        public void SendData(byte[] data)
        {
            if (IsConnected)
            {
                _sct.data2SendQueue.Enqueue(data);
            }
        }
    }

    /// <summary>
    /// 套接字连接线程
    /// </summary>
    class SocketConnectionThread
    {
        /// <summary>
        /// 线程睡眠时间
        /// </summary>
        int _sleepTime = 1000;

        /// <summary>
        /// 主机地址
        /// </summary>
        string _host = "";

        /// <summary>
        /// 主机端口
        /// </summary>
        int _port = 0;

        /// <summary>
        /// TCP客户端
        /// </summary>
        TcpClient _tcpClient;

        /// <summary>
        /// 状态队列「线程安全」
        /// </summary>
        public Queue stateQueue;

        /// <summary>
        /// 要发送的数据队列「线程安全」
        /// </summary>
        public Queue data2SendQueue;
        /// <summary>
        /// 收到的数据队列「线程安全」
        /// </summary>
        public Queue dataReceivedQueue;

        /// <summary>
        /// 网络数据流
        /// </summary>
        NetworkStream _stream;

        /// <summary>
        /// 是否已连接
        /// </summary>
        public bool isConnected = false;

        bool _dispose = false;

        /// <summary>
        /// 字节缓冲
        /// </summary>
        byte[] _buffer;



        public SocketConnectionThread(string host, int port, int sleepTime)
        {
            _buffer = new byte[4096];
            //线程安全处理
            data2SendQueue = Queue.Synchronized(new Queue());
            dataReceivedQueue = Queue.Synchronized(new Queue());
            stateQueue = Queue.Synchronized(new Queue());

            _host = host;
            _port = port;
            _sleepTime = sleepTime;

            Thread t = new Thread(Connect);
            t.IsBackground = true;
            t.Start();
        }

        /// <summary>
        /// 销毁线程（同时会断开连接）
        /// </summary>
        public void Dispose()
        {
            _dispose = true;
        }

        void Connect()
        {
            try
            {
                _tcpClient = new TcpClient();
                IAsyncResult iar = _tcpClient.BeginConnect(_host, _port, null, null);
                iar.AsyncWaitHandle.WaitOne();
                if (_tcpClient.Connected)
                {
                    _tcpClient.EndConnect(iar);
                    stateQueue.Enqueue(ESocketConnectionState.CONNECTED);
                    isConnected = true;
                    _stream = _tcpClient.GetStream();

                    while (IsConnectionAlive())
                    {
                        SendDataHandle();
                        ReceiveDataHandle();
                        Thread.Sleep(_sleepTime);
                    }

                    _stream.Close();
                    isConnected = false;
                }
                _tcpClient.Close();
            }
            catch (Exception e)
            {
                Log.I("网络连接错误！");
            }

            _stream = null;
            _tcpClient = null;
            stateQueue.Enqueue(ESocketConnectionState.DISCONNECTED);
        }

        /// <summary>
        /// 接收数据的处理
        /// </summary>
        void ReceiveDataHandle()
        {
            //判断有数据再读，否则Read会阻塞线程。后面的业务逻辑无法处理
            if (_stream.DataAvailable)
            {
                int size = _stream.Read(_buffer, 0, _buffer.Length);
                if (size > 0)
                {
                    byte[] data = new byte[size];
                    Array.ConstrainedCopy(_buffer, 0, data, 0, size);
                    dataReceivedQueue.Enqueue(data);
                }
            }
        }

        /// <summary>
        /// 发送数据的处理
        /// </summary>
        void SendDataHandle()
        {
            bool needFlush = false;
            while (data2SendQueue.Count > 0)
            {
                byte[] data = data2SendQueue.Dequeue() as byte[];
                _stream.Write(data, 0, data.Length);
                needFlush = true;
            }

            if (needFlush)
            {
                _stream.Flush();
            }
        }

        /// <summary>
        /// 是否连接还活跃
        /// </summary>
        /// <returns></returns>
        bool IsConnectionAlive()
        {
            if (_dispose)
            {
                return false;
            }

            //民间的方法
            bool isOnline = !((_tcpClient.Client.Poll(1000, SelectMode.SelectRead) && (_tcpClient.Client.Available == 0)) || !_tcpClient.Client.Connected);
            if (false == isOnline)
            {
                return false;
            }

            return true;
        }
    }
}
