using System;
using System.DrawingCore;
using System.DrawingCore.Imaging;
using System.IO;

namespace HZC.Utils
{
    /// <summary>
    /// Base64图片操作类
    /// </summary>
    public static class Base64ImageUtil
    {
        /// <summary>
        /// Base64字符串转JPEG
        /// </summary>
        /// <param name="inputStr"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool Base64StringToImage(string inputStr, string filePath)
        {
            try
            {
                string dummyData = inputStr.Trim().Replace("%", "").Replace(",", "").Replace(" ", "+");
                if (dummyData.Length % 4 > 0)
                {
                    dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');
                }
                byte[] arr = Convert.FromBase64String(dummyData);
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);

                var localPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(localPath))
                {
                    Directory.CreateDirectory(localPath);
                }

                bmp.Save(filePath, ImageFormat.Jpeg);
                ms.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 根据图片路径获取图片后转Base64
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ImageToBase64(string filePath)
        {
            MemoryStream m = new MemoryStream();
            Bitmap bp = new Bitmap(filePath);
            bp.Save(m, bp.RawFormat);
            byte[] b = m.GetBuffer();
            string base64string = Convert.ToBase64String(b);
            return base64string;
        }

        /// <summary>
        /// Bitmap转Base64
        /// </summary>
        /// <param name="img">要转换的图片</param>
        /// <returns></returns>
        public static string ImageToBase64(Bitmap img)
        {
            MemoryStream m = new MemoryStream();
            img.Save(m, img.RawFormat);
            byte[] b = m.GetBuffer();
            string base64string = Convert.ToBase64String(b);
            return base64string;
        }
    }
}
