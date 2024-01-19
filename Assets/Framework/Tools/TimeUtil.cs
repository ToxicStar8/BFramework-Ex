/*********************************************
 * BFramework
 * 时间工具类
 * 创建时间：2024/01/19 9:40:00
 *********************************************/
using System;

namespace Framework
{
    public class TimeUtil
    {
        /// <summary>
        /// 获取当前秒时间戳
        /// </summary>
        public static long GetNowTimeSeconds()
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        /// <summary>
        /// 获取当前毫秒时间戳
        /// </summary>
        public static long GetNowTimeMilliseconds()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// 时间戳转日期
        /// </summary>
        public static DateTime TimestampToDate(long timestamp)
        {
            var dt = new DateTime(1970, 1, 1, 8, 0, 0) + TimeSpan.FromSeconds(timestamp);
            return dt;
        }

        /// <summary>
        /// 日期转时间戳
        /// </summary>
        public static long DateToTimestamp(DateTime nowDateTime)
        {
            DateTime dt = new DateTime(1970, 1, 1, 8, 0, 0);
            long timeStamp = Convert.ToInt32(nowDateTime.Subtract(dt).TotalSeconds);
            return timeStamp;
        }
    }
}
