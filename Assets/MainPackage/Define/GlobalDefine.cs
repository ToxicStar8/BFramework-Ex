/*********************************************
 * BFramework
 * 游戏入口
 * 创建时间：2023/06/16 16:54:23
 *********************************************/
using Obfuz;
using Obfuz.EncryptionVM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using YooAsset;

namespace MainPackage
{
    /// <summary>
    /// 主包全局变量定义
    /// </summary>
    public static class GlobalDefine
    {
        /// <summary>
        /// YooAsset包名
        /// </summary>
        public static readonly string PackageName = "DefaultPackage";

        /// <summary>
        /// 热更的DLL名
        /// </summary>
        public static readonly string HotfixDllName = "Assembly-CSharp.dll";

        /// <summary>
        /// 资源下载地址
        /// </summary>
        public static readonly string DebugServerCDNUrl1 = "www.baidu1.com";
        public static readonly string DebugServerCDNUrl2 = "www.baidu2.com";
        public static readonly string ReleaseServerCDNUrl1 = "www.baidu1.com";
        public static readonly string ReleaseServerCDNUrl2 = "www.baidu2.com";

    }
}