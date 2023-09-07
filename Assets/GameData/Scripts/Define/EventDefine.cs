/*********************************************
 * BFramework
 * 游戏事件枚举
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameData
{
    /// <summary>
    /// 游戏事件 每100一个模块
    /// </summary>
    public enum GameEvent
    {
        GameTest = 0,
    }

    /// <summary>
    /// UI事件 每100一个模块
    /// </summary>
    public enum UIEvent
    {
        OnUIBattleMainDrawCard = 10000,         //抽卡
        OnUIBattleMainChangeRound,              //切换回合
    }

    /// <summary>
    /// Module事件 每100一个模块
    /// </summary>
    public enum ModuleEvent
    {
        ModuleTest = 20000,
    }
}
