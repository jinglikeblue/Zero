using System;
using System.Net;
using System.Text;

namespace Jing
{
    /// <summary>
    /// 字节数组操作
    /// </summary>
    public class ByteArray
    {
        /// <summary>
        /// byte类型占用字节数
        /// </summary>
        public const byte BYTE_SIZE = 1;

        /// <summary>
        /// char类型占用字节数
        /// </summary>
        public const byte CHAR_SIZE = 2;

        /// <summary>
        /// float类型占用字节数
        /// </summary>
        public const byte FLOAT_SIZE = 4;

        /// <summary>
        /// double类型占用字节数
        /// </summary>
        public const byte DOUBLE_SIZE = 8;

        /// <summary>
        /// short类型占用字节数
        /// </summary>
        public const byte SHORT_SIZE = 2;

        /// <summary>
        /// ushort类型占用字节数
        /// </summary>
        public const byte USHORT_SIZE = 2;

        /// <summary>
        /// int类型占用字节数
        /// </summary>
        public const byte INT_SIZE = 4;

        /// <summary>
        /// uint类型占用字节数
        /// </summary>
        public const byte UINT_SIZE = 4;

        /// <summary>
        /// long类型占用字节数
        /// </summary>
        public const byte LONG_SIZE = 8;

        /// <summary>
        /// ulong类型占用字节数
        /// </summary>
        public const byte ULONG_SIZE = 8;

        /// <summary>
        /// 默认使用的文本编码(UTF8，全局生效，可根据需要修改）
        /// </summary>
        public static Encoding defaultEncoding = Encoding.UTF8;

        /// <summary>
        /// 默认的缓冲区大小(65535，全局生效，可根据需要修改）
        /// </summary>
        public static int defaultBufferSize = 65535;

        /// <summary>
        /// 字节数组
        /// </summary>
        public byte[] Bytes { get; private set; }

        /// <summary>
        /// 定义的字节数组大小
        /// </summary>
        public int Size
        {
            get
            {
                return Bytes.Length;
            }
        }

        /// <summary>
        /// 字节序是否是大端
        /// </summary>
        bool _isBigEndian;

        /// <summary>
        /// 是否需要转换字节序
        /// </summary>
        bool _isNeedConvertEndian = false;

        /// <summary>
        /// 字节数组操作标识符位置
        /// </summary>
        public int Pos { get; private set; } = 0;

        /// <summary>
        /// 目前有效字节大小
        /// </summary>
        public int Available { get; private set; } = 0;

        /// <summary>
        /// 剩余可写入的数据长度
        /// </summary>
        public int WriteEnableSize
        {
            get
            {
                return Size - Available;
            }
        }

        /// <summary>
        /// 根据Available以及Pos计算出的剩余可读取的数据长度
        /// </summary>
        public int ReadEnableSize
        {
            get
            {
                return Available - Pos;
            }
        }

        /// <summary>
        /// 将有效数据转为字节数组导出
        /// </summary>
        /// <returns></returns>
        public byte[] GetAvailableBytes()
        {
            byte[] bytes = new byte[Available];
            Array.Copy(Bytes, 0, bytes, 0, Available);
            return bytes;
        }

        public ByteArray(byte[] bytes, int available, bool isBigEndian = true)
        {
            Init(bytes, isBigEndian);
            Available = available;
        }

        public ByteArray(byte[] bytes, bool isBigEndian = true)
        {
            Init(bytes, isBigEndian);
            Available = bytes.Length;
        }

        public ByteArray(bool isBigEndian = true)
        {
            Init(new byte[defaultBufferSize], isBigEndian);
        }

        public ByteArray(int bufferSize, bool isBigEndian = true)
        {                       
            Init(new byte[bufferSize], isBigEndian);            
        }

        void Init(byte[] bytes, bool isBigEndian = true)
        {
            _isBigEndian = isBigEndian;
            if(isBigEndian != BitConverter.IsLittleEndian)
            {
                _isNeedConvertEndian = true;
            }
            Bytes = bytes;
            SetPos(0);
        }

        /// <summary>
        /// 重置对象的数据，指针和可读数据都会被初始化
        /// </summary>
        public void Reset()
        {
            if(Bytes.Length != defaultBufferSize)
            {
                Bytes = new byte[defaultBufferSize];
            }
            Available = 0;
            SetPos(0);
        }

        /// <summary>
        /// 移动指针位置
        /// </summary>
        /// <param name="v">移动的偏移值</param>
        public void MovePos(int v)
        {            
            SetPos(Pos + v);
        }

        /// <summary>
        /// 设置指针位置
        /// </summary>
        /// <param name="v">指针的位置</param>
        public void SetPos(int v)
        {
            Pos = v;
        }


        #region write
        public void Write(short v)
        {
            if(_isNeedConvertEndian)
            {
                v = IPAddress.HostToNetworkOrder(v);
            }

            Write(BitConverter.GetBytes(v));
        }

        public void Write(ushort v)
        {
            Write((short)v);
        }

        public void Write(int v)
        {
            if (_isNeedConvertEndian)
            {                
                v = IPAddress.HostToNetworkOrder(v);
            }

            Write(BitConverter.GetBytes(v));            
        }

        public void Write(uint v)
        {
            Write((int)v);
        }

        public void Write(long v)
        {
            if (_isNeedConvertEndian)
            {
                v = IPAddress.HostToNetworkOrder(v);
            }

            Write(BitConverter.GetBytes(v));
        }

        public void Write(ulong v)
        {
            Write((long)v);
        }

        public void Write(float v)
        {
            Write(BitConverter.GetBytes(v));
        }

        public void Write(double v)
        {
            Write(BitConverter.GetBytes(v));
        }

        public void Write(char v)
        {
            Write(BitConverter.GetBytes(v));
        }

        /// <summary>
        /// 以默认格式写入字符串数据体,并且会在数据体前面写入一个ushort来表示字符串数据体的长度。
        /// </summary>
        /// <param name="v"></param>
        public void Write(string v)
        {
            Write(v, defaultEncoding);
        }

        /// <summary>
        /// 以指定格式写入字符串数据体,并且会在数据体前面写入一个ushort来表示字符串数据体的长度。
        /// </summary>
        /// <param name="v"></param>
        /// <param name="encoding"></param>
        public void Write(string v, Encoding encoding)
        {
            byte[] stringBytes = encoding.GetBytes(v);
            ushort stringSize = (ushort)stringBytes.Length;
            Write(stringSize);
            Write(stringBytes);
        }

        /// <summary>
        /// 直接以默认格式写入字符串编码数据
        /// </summary>
        /// <param name="v"></param>
        public void WriteStringBytes(string v)
        {
            WriteStringBytes(v, defaultEncoding);
        }  

        /// <summary>
        /// 直接以指定格式写入字符串的编码数据
        /// </summary>
        /// <param name="v"></param>
        /// <param name="encoding"></param>
        public void WriteStringBytes(string v, Encoding encoding)
        {
            byte[] stringBytes = encoding.GetBytes(v);
            Write(stringBytes);
        }

        public void Write(byte v)
        {
            Bytes[Pos] = v;
            Available += 1;
            MovePos(1);
        }

        public void Write(byte[] sourceBytes)
        {
            Write(sourceBytes, 0, sourceBytes.Length);            
        }

        /// <summary>
        /// 将目标字节数组从sourceIndex指定的位置，读取length长度写入
        /// </summary>
        /// <param name="sourceBytes">读取的字节数组</param>
        /// <param name="sourceIndex">读取开始的位置</param>
        /// <param name="length">读取的长度</param>
        /// <returns></returns>
        public void Write(byte[] sourceBytes, int sourceIndex, int length)
        {
            Array.Copy(sourceBytes, sourceIndex, Bytes, Pos, length);
            Available += length;
            MovePos(length);
        }

        #endregion


        #region read
        public short ReadShort()
        {
            short v = BitConverter.ToInt16(Bytes, Pos);
            if (_isNeedConvertEndian)
            {
                v = IPAddress.NetworkToHostOrder(v);
            }
            MovePos(SHORT_SIZE);
            return v;
        }

        public ushort ReadUShort()
        {
            return (ushort)ReadShort();
        }

        public int ReadInt()
        {
            int v = BitConverter.ToInt32(Bytes, Pos);
            if (_isNeedConvertEndian)
            {
                v = IPAddress.NetworkToHostOrder(v);
            }
            MovePos(INT_SIZE);
            return v;
        }

        public uint ReadUInt()
        {
            return (uint)ReadInt();
        }

        public long ReadLong()
        {
            long v = BitConverter.ToInt64(Bytes, Pos);
            if (_isNeedConvertEndian)
            {
                v = IPAddress.NetworkToHostOrder(v);
            }
            MovePos(LONG_SIZE);
            return v;
        }

        public ulong ReadULong()
        {
            return (ulong)ReadLong();
        }

        public float ReadFloat()
        {            
            float v = BitConverter.ToSingle(Bytes, Pos);
            MovePos(FLOAT_SIZE);
            return v;
        }

        public double ReadDouble()
        {
            double v = BitConverter.ToDouble(Bytes, Pos);
            MovePos(DOUBLE_SIZE);
            return v;
        }

        public char ReadChar()
        {
            char v = BitConverter.ToChar(Bytes, Pos);
            MovePos(CHAR_SIZE);
            return v;
        }

        /// <summary>
        /// 以默认格式读取字符串数据体,并且会在数据体前面读取一个ushort来表示字符串数据体的长度。
        /// </summary>
        /// <param name="v"></param>
        /// <param name="encoding"></param>
        public string ReadString()
        {
            return ReadString(defaultEncoding);
        }

        /// <summary>
        /// 以指定格式读取字符串数据体,并且会在数据体前面读取一个ushort来表示字符串数据体的长度。
        /// </summary>
        /// <param name="v"></param>
        /// <param name="encoding"></param>
        public string ReadString(Encoding encoding)
        {
            ushort stringSize = ReadUShort();
            string v = encoding.GetString(Bytes, Pos, stringSize);
            MovePos(stringSize);
            return v;
        }

        /// <summary>
        /// 直接以默认格式读取字符串编码数据
        /// </summary>
        /// <param name="v"></param>
        public string ReadStringBytes(int length)
        {
            return ReadStringBytes(defaultEncoding, length);
        }

        /// <summary>
        /// 直接以指定格式读取字符串编码数据
        /// </summary>
        /// <param name="v"></param>
        public string ReadStringBytes(Encoding encoding, int length)
        {
            string v = encoding.GetString(Bytes, Pos, length);
            MovePos(length);
            return v;
        }

        public byte ReadByte()
        {
            byte v = Bytes[Pos];
            MovePos(BYTE_SIZE);
            return v;
        }

        public byte[] ReadBytes(int length)
        {
            byte[] bytes = new byte[length];
            Array.Copy(Bytes, Pos, bytes, 0, length);
            MovePos(length);
            return bytes;
        }
        #endregion
    }
}