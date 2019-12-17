using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Zero
{
    /// <summary>
    /// 加解密工具类
    /// </summary>
    public class CryptoUtility
    {
        /// <summary>
        /// 得到32位字符串的MD5值
        /// </summary>
        /// <param name="input">输入的字符串</param>
        /// <param name="isUpper">MD5是否是大写字母</param>
        /// <returns></returns>
        public static string Get32ByteMD5(string input, bool isUpper = true)
        {
            using (MD5 mi = MD5.Create())
            {
                byte[] buffer = Encoding.Default.GetBytes(input);
                //开始加密
                byte[] newBuffer = mi.ComputeHash(buffer);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < newBuffer.Length; i++)
                {
                    var hex = isUpper ? "X2" : "x2";
                    sb.Append(newBuffer[i].ToString(hex));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// 得到16位字符串的MD5值
        /// </summary>
        /// <param name="input">输入的字符串</param>
        /// <param name="isUpper">MD5是否是大写字母</param>
        /// <returns></returns>
        public static string Get16ByteMD5(string input, bool isUpper = true)
        {
            var md5 = Get32ByteMD5(input, isUpper);
            return md5.Substring(8, 16);
        }

        static byte[] GenerateAESKey(string key)
        {
            key = Get16ByteMD5(key);
            byte[] keyBytes = Encoding.ASCII.GetBytes(key);
            byte[] key128Bit = new byte[16];
            Array.Copy(keyBytes, 0, key128Bit, 0, keyBytes.Length);
            return key128Bit;
        }

        /// <summary>
        /// AES加密UTF8字符串
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns>base64字符串</returns>
        public static string AESEncryptString(string input, string key, string iv = null)
        {
            var data = Encoding.UTF8.GetBytes(input);
            var encrypted = AESEncrypt(data, key, iv);
            var base64 = Convert.ToBase64String(encrypted);
            return base64;
        }

        /// <summary>
        /// AES解密UTF8字符串
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string AESDecryptString(string input, string key, string iv = null)
        {
            var encrypted = Convert.FromBase64String(input);
            var data = AESDecrypt(encrypted, key, iv);
            return Encoding.UTF8.GetString(data);            
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static byte[] AESEncrypt(byte[] input, string key, string iv = null)
        {
            return new AesHelper(input, key, iv).Encrypt();
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static byte[] AESDecrypt(byte[] input, string key, string iv = null)
        {
            return new AesHelper(input, key, iv).Decrypt();
        }
    }
}
