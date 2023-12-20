/*********************************************
 * BFramework
 * 游戏常量
 * 创建时间：2023/04/03 14:13:23
 *********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameData
{
    /// <summary>
    /// 游戏常量
    /// </summary>
    public static class ConstDefine
    {
        public const string ServerHttp = "http://toxicstar.top/";           //服务端Http地址
        public const string ServerWS = "ws://175.178.9.175:8801";           //服务端WebSocket地址

        public const string DrawCardTimeKey = "DrawCardTimeKey";            //抽卡定时器Key
        public const string CreateMonsterTimeKey = "CreateMonsterTimeKey";  //刷怪定时器Key
    }
}
