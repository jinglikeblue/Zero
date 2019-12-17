using System;
using System.Security.Cryptography;
using System.Text;

namespace Zero
{
    /// <summary>
    /// AES加密辅助类
    /// </summary>
    public sealed class AesHelper
    {
        public byte[] input { get; private set; }
        public string key { get; private set; }
        public string iv { get; private set; }

        public AesHelper(byte[] input, string key, string iv = null)
        {
            if (input == null || input.Length <= 0)
            {
                throw new ArgumentNullException("input");
            }

            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            this.input = input;
            this.key = key;
            this.iv = iv;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <returns></returns>
        public byte[] Encrypt()
        {
            byte[] output;

            using (Aes aes = Aes.Create())
            {
                aes.Padding = PaddingMode.Zeros;
                aes.Key = GenerateKey();
                aes.IV = GenerateIV(aes);

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                output = encryptor.TransformFinalBlock(input, 0, input.Length);
            }

            return output;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <returns></returns>
        public byte[] Decrypt()
        {
            byte[] output;
            using (Aes aes = Aes.Create())
            {
                aes.Padding = PaddingMode.Zeros;
                aes.Key = GenerateKey();
                aes.IV = GenerateIV(aes);

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                var tempOutput = decryptor.TransformFinalBlock(input, 0, input.Length);

                int outputSize = tempOutput.Length;
                //精简输出块
                for (int i = 0; i < tempOutput.Length; i++)
                {
                    if (tempOutput[i] == '\0')
                    {
                        outputSize = i;
                        break;
                    }
                }
                output = new byte[outputSize];
                Array.Copy(tempOutput, 0, output, 0, outputSize);
            }
            return output;
        }

        byte[] GenerateKey()
        {
            byte[] keyBytes = Encoding.ASCII.GetBytes(key);
            byte[] key128Bit = new byte[16];
            var copyLength = keyBytes.Length < key128Bit.Length ? keyBytes.Length : key128Bit.Length;
            Array.Copy(keyBytes, 0, key128Bit, 0, copyLength);
            return key128Bit;
        }

        byte[] GenerateIV(SymmetricAlgorithm sa)
        {
            if(null == iv)
            {
                iv = "aes";
            }

            var tempBytes = Encoding.ASCII.GetBytes(iv);
            var ivBytes = new byte[sa.BlockSize >> 3];
            var copyLength = tempBytes.Length < ivBytes.Length ? tempBytes.Length : ivBytes.Length;
            Array.Copy(tempBytes, 0, ivBytes, 0, copyLength);
            return ivBytes;
        }
    }
}
