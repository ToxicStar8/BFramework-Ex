/*********************************************
 * BFramework
 * 框架常量
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Framework
{
    /// <summary>
    /// 常量
    /// </summary>
    public static class ConstDefine
    {
        public const string ServerHttp = "http://175.178.9.175:5140/";      //服务端Http地址
        public const string ServerWS = "ws://121.40.165.18:8800";           //服务端WebSocket地址
        public const string DownloadUrl = "toxicstar.top/Test/CDN/";        //项目的AB包下载地址
        public const string ABInfoName = "jsoninformation";                 //AB包索引信息名（资源名以及AB包的依赖信息，一表搞定）
        public const string ABMd5InfoName = "fileUpdateInfo.json";          //AB包MD5信息名（用于比对需要更新的AB包）
        public const string HotfixDllName = "Assembly-CSharp.dll";          //热更的DLL名
        public const string VolumBackground = "VolumBackground";            //背景音乐音量
        public const string VolumSound = "VolumSound";                      //音效音量
        public const string ABConfigPath = "Assets/Editor/ABConfig/ABConfig.asset";   //ABConfig配置文件位置
        public const string JsoninformationDirPath = "Assets/Editor/ABConfig/JsonInformation/";   //依赖信息Json文件夹的路径
        public const string JsoninformationPath = JsoninformationDirPath + ABInfoName + ".json";   //依赖信息Json的路径
    }
}
