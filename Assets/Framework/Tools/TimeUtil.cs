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
        /// 时间戳（毫秒）转日期
        /// </summary>
        public static DateTime TimestampMillisecondsToDate(long timestamp)
        {
            var dt = new DateTime(1970, 1, 1, 8, 0, 0, 0) + TimeSpan.FromMilliseconds(timestamp);
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

        /// <summary>
        /// 日期转时间戳（毫秒）
        /// </summary>
        public static long DateToTimestampMilliseconds(DateTime nowDateTime)
        {
            DateTime dt = new DateTime(1970, 1, 1, 8, 0, 0, 0);
            long timeStamp = Convert.ToInt32(nowDateTime.Subtract(dt).TotalMilliseconds);
            return timeStamp;
        }

        /// <summary>
        /// 时间戳转字符串
        /// </summary>
        public static string TimestampToStr(long timestamp, bool isShowHour = false)
        {
            int h = (int)(timestamp / 3600);        // 计算小时数
            int m = (int)(timestamp % 3600 / 60);   // 计算分钟数
            int s = (int)(timestamp % 60);          // 计算秒数

            if (!isShowHour)
            {
                return $"{m.ToString("D2")}:{s.ToString("D2")}";
            }
            return $"{h.ToString("D2")}:{m.ToString("D2")}:{s.ToString("D2")}";
        }
    }
}
