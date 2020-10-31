using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Jing
{
    public class MD5Utility
    {
        /// <summary>
        /// MD5加密(16位)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetShortMD5(string input, bool isUpper = false)
        {
            var longMD5 = GetMD5(input, isUpper);            
            return longMD5.Substring(8, 16);
        }

        /// <summary>
        /// MD5加密(16位)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetShortMD5(Stream input, bool isUpper = false)
        {
            var longMD5 = GetMD5(input, isUpper);
            return longMD5.Substring(8, 16);
        }


        /// <summary>
        /// MD5加密(32位)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5(string input, bool isUpper = false)
        {
            var format = isUpper ? "X2" : "x2";
            using (MD5 mi = MD5.Create())
            {
                byte[] buffer = Encoding.Default.GetBytes(input);
                //开始加密
                byte[] newBuffer = mi.ComputeHash(buffer);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < newBuffer.Length; i++)
                {
                    sb.Append(newBuffer[i].ToString(format));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// MD5加密(32位)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5(Stream input, bool isUpper = false)
        {
            var format = isUpper ? "X2" : "x2";
            using (MD5 mi = MD5.Create())
            {
                //开始加密
                byte[] newBuffer = mi.ComputeHash(input);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < newBuffer.Length; i++)
                {
                    sb.Append(newBuffer[i].ToString(format));
                }
                return sb.ToString();
            }
        }
    }
}
