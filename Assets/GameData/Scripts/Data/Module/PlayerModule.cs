/*********************************************
 * 
 * 脚本名：StorageModule.cs
 * 创建时间：2023/04/06 11:45:09
 *********************************************/
using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    /// <summary>
    /// 玩家数据操作
    /// </summary>
    public class PlayerModule : ModuleBase
    {
        /// <summary>
        /// 开始新游戏
        /// </summary>
        public static void FnStartNewGame(Action callback)
        {
            GameGod.Instance.DataManager.GetNewData();
            callback?.Invoke();
        }
    }
}
