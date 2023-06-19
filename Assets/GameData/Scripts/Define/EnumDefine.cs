/*********************************************
 * BFramework
 * 游戏数据枚举
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameData
{
    /// <summary>
    /// 主角类型
    /// </summary>
    public enum E_RoleType
    {
        None = 0,           //无
        Green = 1,          //绿龙
        Yellow = 2,         //黄龙
        Blue = 3,           //蓝龙
        Red = 4,            //红龙
    }

    /// <summary>
    /// 花色类型
    /// </summary>
    public enum E_SuitType
    {
        None = 0,           //无
        Spade = 1,          //黑桃
        Heart = 2,          //红心
        Club = 3,           //梅花
        Diamond = 4,        //方块
        Joker = 5,          //大小王
    }
}
