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

        /// <summary>
        /// 选择角色
        /// </summary>
        /// <param name="obj"></param>
        public static void FnSelectRole(E_RoleType roleId, Action callback)
        {
            PlayData.RoleData.SetRoleId((int)roleId);
            callback?.Invoke();
        }

        /// <summary>
        /// 抽卡
        /// </summary>
        /// <param name="obj"></param>
        public static void FnDrawCard(Action callback)
        {
            callback?.Invoke();
            //抽完卡之后增加Idx
            PlayData.CurSelectIdx++;
        }

        /// <summary>
        /// 重置牌序
        /// </summary>
        /// <param name="obj"></param>
        public static void FnReSortCardList(Action callback)
        {
            PlayData.CurSelectIdx = 0;
            PlayData.CardHeldList.SortByDisordered();
            callback?.Invoke();
        }
    }
}
