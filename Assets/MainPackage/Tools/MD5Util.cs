/*********************************************
 * BFramework
 * Md5工具
 * 创建时间：2022/12/29 20:13:41
 *********************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace MainPackage
{
    /// <summary>
    /// Md5工具（非安全用途，仅做一致性校验）
    /// </summary>
    public static class MD5Util
    {
        /// <summary>
        /// 根据路径获得文件 Md5（大写十六进制）
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public static string GetMd5ByPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("文件路径不能为空", nameof(filePath));
            if (!File.Exists(filePath))
                throw new FileNotFoundException("文件不存在", filePath);

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return ComputeHash(fs);
            }
        }

        /// <summary>
        /// 根据字节数组获得 Md5（大写十六进制）
        /// </summary>
        public static string GetMd5ByBytes(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(bytes);
                return BytesToHex(hash);
            }
        }

        /// <summary>
        /// 根据字符串（UTF8）获得 Md5（大写十六进制）
        /// </summary>
        public static string GetMd5ByString(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            var bytes = Encoding.UTF8.GetBytes(text);
            return GetMd5ByBytes(bytes);
        }

        #region 内部工具
        private static string ComputeHash(Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(stream);
                return BytesToHex(hash);
            }
        }

        private static string BytesToHex(byte[] hash)
        {
            // 16 bytes => 32 hex chars
            char[] c = new char[hash.Length * 2];
            int ci = 0;
            for (int i = 0; i < hash.Length; i++)
            {
                byte b = hash[i];
                int hi = b >> 4;
                int lo = b & 0x0F;
                c[ci++] = (char)(hi < 10 ? ('0' + hi) : ('a' + hi - 10));
                c[ci++] = (char)(lo < 10 ? ('0' + lo) : ('a' + lo - 10));
            }
            return new string(c).ToString();
        }
        #endregion
    }
}
