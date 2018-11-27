using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HZC.Utils
{
    public class RSAEncryptUtil
    {
        #region 公钥加密
        public static string RsaEncrypt(string rawInput, string publicKey)
        {
            try
            {
                if (string.IsNullOrEmpty(rawInput))
                {
                    return string.Empty;
                }

                if (string.IsNullOrEmpty(publicKey))
                {
                    throw new ArgumentException("Invalid Public Key");
                }

                using (var rsaProvider = new RSACryptoServiceProvider())
                {
                    //有含义的字符串转化为字节流
                    var inputBytes = Encoding.UTF8.GetBytes(rawInput);
                    //载入公钥
                    rsaProvider.FromXmlString(publicKey);
                    //单块最大长度
                    int bufferSize = (rsaProvider.KeySize / 8) - 11;
                    var buffer = new byte[bufferSize];
                    using (MemoryStream inputStream = new MemoryStream(inputBytes), outputStream = new MemoryStream())
                    {
                        while (true)
                        { 
                            //分段加密
                            int readSize = inputStream.Read(buffer, 0, bufferSize);
                            if (readSize <= 0)
                            {
                                break;
                            }

                            var temp = new byte[readSize];
                            Array.Copy(buffer, 0, temp, 0, readSize);
                            var encryptedBytes = rsaProvider.Encrypt(temp, false);
                            outputStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                        }
                        return Convert.ToBase64String(outputStream.ToArray());//转化为字节流方便传输
                    }
                }
            }
            catch
            {
                return "";
            }
        }
        #endregion
    }

}
