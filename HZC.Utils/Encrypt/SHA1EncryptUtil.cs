using System;
using System.Security.Cryptography;
using System.Text;

namespace HZC.Utils
{
    public static class SHA1EncryptUtil
    {
        /// <summary>
        /// 加密字符串，默认编码Encoding.UTF8
        /// </summary>
        /// <param name="content">要加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string Encrypt(string content)
        {
            return Encrypt(content, Encoding.UTF8);
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="content">要加密的字符串</param>
        /// <param name="encode">字符串编码</param>
        /// <returns>加密后的字符串</returns>
        public static string Encrypt(string content, Encoding encode)
        {
            try
            {
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                byte[] bytes_in = encode.GetBytes(content);
                byte[] bytes_out = sha1.ComputeHash(bytes_in);
                sha1.Dispose();

                string result = BitConverter.ToString(bytes_out);
                result = result.Replace("-", "");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("SHA1加密失败：" + ex.Message);
            }
        }
    }
}
