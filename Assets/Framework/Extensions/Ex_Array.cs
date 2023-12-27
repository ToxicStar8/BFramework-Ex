/*********************************************
 * BFramework
 * 数组方法扩展类
 * 创建时间：2023/12/27 11:25:23
 *********************************************/
using LitJson;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 数组扩展类
    /// </summary>
    public static class Ex_Array
    {
        public static string ToJson<T>(this T[] array)
        {
            return JsonMapper.ToJson(array);
        }

        /// <summary>
        /// 转换成Vector格式
        /// </summary>
        public static Vector3 ToVector3(this int[] array)
        {
            Vector3 v3 = new Vector3
            {
                x = array[0],
                y = array[1],
                z = array[2]
            };
            return v3;
        }

        /// <summary>
        /// 转换成Vector格式
        /// </summary>
        public static Vector3 ToVector3(this float[] array)
        {
            Vector3 v3 = new Vector3
            {
                x = array[0],
                y = array[1],
                z = array[2]
            };
            return v3;
        }
    }
}
