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
        /// short类型占用字节数
        /// </summary>
        public const byte SHORT_SIZE = 2;
        /// <summary>
        /// int类型占用字节数
        /// </summary>
        public const byte INT_SIZE = 4;
        /// <summary>
        /// long类型占用字节数
        /// </summary>
        public const byte LONG_SIZE = 8;      

        /// <summary>
        /// 默认使用的文本编码(全局生效）
        /// </summary>
        public static Encoding defaultEncoding = Encoding.UTF8;

        /// <summary>
        /// 默认的缓冲区大小(全局生效）
        /// </summary>
        public static int defaultBufferSize = 65535;

        /// <summary>
        /// 字节数组
        /// </summary>
        byte[] _bytes;

        /// <summary>
        /// 字节序是否是大端
        /// </summary>
        bool _isBigEndian;

        /// <summary>
        /// 是否需要转换字节序
        /// </summary>
        bool _isNeedConvertEndian = false;

        /// <summary>
        /// 字节数组操作位置
        /// </summary>
        int _pos;

        /// <summary>
        /// 目前指针位置
        /// </summary>
        public int Position
        {
            get { return _pos; }
        }        

        /// <summary>
        /// 还可读/写的字节数
        /// </summary>
        public int Available
        {
            get
            {
                return _bytes.Length - _pos;
            }
        }

        /// <summary>
        /// 将数据转为字节数组导出
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            byte[] bytes = new byte[Position];
            Array.Copy(_bytes, 0, bytes, 0, Position);
            return bytes;
        }

        public ByteArray(byte[] bytes, bool isBigEndian = true)
        {
            Init(bytes, isBigEndian);
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
            _bytes = bytes;
            _pos = 0;
        }

        public void Reset()
        {
            _pos = 0;
        }

        /// <summary>
        /// 移动指针
        /// </summary>
        public void MovePos(int v)
        {            
            _pos += v;
        }


        #region write
        public void WriteShort(short v)
        {
            if(_isNeedConvertEndian)
            {
                v = IPAddress.HostToNetworkOrder(v);
            }

            WriteBytes(BitConverter.GetBytes(v));
        }

        public void WriteUShort(ushort v)
        {
            WriteShort((short)v);
        }

        public void WriteInt(int v)
        {
            if (_isNeedConvertEndian)
            {                
                v = IPAddress.HostToNetworkOrder(v);
            }

            WriteBytes(BitConverter.GetBytes(v));            
        }

        public void WriteUInt(uint v)
        {
            WriteInt((int)v);
        }

        public void WriteLong(long v)
        {
            if (_isNeedConvertEndian)
            {
                v = IPAddress.HostToNetworkOrder(v);
            }

            WriteBytes(BitConverter.GetBytes(v));
        }

        public void WriteULong(ulong v)
        {
            WriteLong((long)v);
        }

        public void WriteFloat(float v)
        {
            WriteBytes(BitConverter.GetBytes(v));
        }

        public void WriteChar(char v)
        {
            WriteBytes(BitConverter.GetBytes(v));
        }

        public int WriteString(string v)
        {
            return WriteString(v, defaultEncoding);
        }

        public int WriteString(string v, Encoding encoding)
        {
            return WriteBytes(encoding.GetBytes(v));
        }

        public void WriteByte(byte v)
        {
            _bytes[_pos] = v;
            MovePos(1);
        }

        public int WriteBytes(byte[] bytes)
        {
            bytes.CopyTo(_bytes, _pos);
            MovePos(bytes.Length);
            return bytes.Length;
        }

        #endregion


        #region read
        public short ReadShort()
        {
            short v = BitConverter.ToInt16(_bytes, _pos);
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
            int v = BitConverter.ToInt32(_bytes, _pos);
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
            long v = BitConverter.ToInt64(_bytes, _pos);
            if (_isNeedConvertEndian)
            {
                v = IPAddress.NetworkToHostOrder(v);
            }
            MovePos(LONG_SIZE);
            return v;
        }

        public ulong ReadULong()
        {
            return (ulong)ReadULong();
        }

        public float ReadFloat()
        {            
            float v = BitConverter.ToSingle(_bytes, _pos);
            MovePos(FLOAT_SIZE);
            return v;
        }

        public char ReadChar()
        {
            char v = BitConverter.ToChar(_bytes, _pos);
            MovePos(CHAR_SIZE);
            return v;
        }

        public string ReadString(int length)
        {
            return ReadString(defaultEncoding, length);
        }

        public string ReadString(Encoding encoding, int length)
        {
            string v = encoding.GetString(_bytes, _pos, length);
            MovePos(length);
            return v;
        }

        public byte ReadByte()
        {
            byte v = _bytes[_pos];
            MovePos(BYTE_SIZE);
            return v;
        }

        public byte[] ReadBytes(int length)
        {
            byte[] bytes = new byte[length];
            Array.Copy(_bytes, _pos, bytes, 0, length);
            MovePos(length);
            return bytes;
        }
        #endregion
    }
}