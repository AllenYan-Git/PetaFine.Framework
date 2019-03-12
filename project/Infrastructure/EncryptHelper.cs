using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class EncryptHelper
    {
        /// <summary>
        /// MD5 hash加密
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static string MD5(string s)
        {
            var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var result = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(s.Trim())));
            result = result.Replace("-", "");
            return result;
        }
    }
}
