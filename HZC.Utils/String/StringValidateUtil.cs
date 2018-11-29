using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace HZC.Utils
{
    public static class StringValidateUtil
    {
        /// <summary>
        /// 是否有效的手机号码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMobile(string input)
        {
            return Regex.IsMatch(input, @"^1[3|4|5|6|7|8|9]\d{10}$");
        }

        /// <summary>
        /// 是否有效的微信openid
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsOpenId(string input)
        {
            return Regex.IsMatch(input, @"^[a-zA-Z\d_]{5,}$");
        }

        /// <summary>
        /// 是否有效的url地址
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsUrl(string input)
        {
            return Regex.IsMatch(input, @"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&:/~\+#]*[\w\-\@?^=%&/~\+#])?");
        }
    }
}
