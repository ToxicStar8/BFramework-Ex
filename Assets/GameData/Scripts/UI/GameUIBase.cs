/*********************************************
 * BFramework
 * UI通用方法存放基类
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using Framework;
using MainPackage;
using System;
using UnityEngine;

namespace GameData
{
    /// <summary>
    /// UI通用方法存放基类
    /// </summary>
    public abstract class GameUIBase : UIBase
    {
        public void AddEventListener(UIEvent eventNo, Action<object[]> callback,string remark = null)
        {
            AddEventListener((ushort)eventNo, callback);
        }
        public void SendEvent(UIEvent eventNo, params object[] args)
        {
            SendEvent((ushort)eventNo, args);
        }

        /// <summary>
        /// 显示新增道具
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="itemCount"></param>
        /// <param name="parent"></param>
        public void ShowAddItemTips(int itemId, int itemCount,RectTransform parent)
        {
            GameGod.Instance.UIManager.OpenUI<UIAddItemTips>(E_UILevel.Tips, itemId, itemCount , parent);
        }

        /// <summary>
        /// 显示确认框
        /// </summary>
        /// <param name="tips"></param>
        public void ShowDialog(UIDialogData dialogData)
        {
            GameGod.Instance.UIManager.OpenUI<UIDialog>(E_UILevel.Pop, dialogData);
        }

        /// <summary>
        /// 显示提示
        /// </summary>
        /// <param name="tips"></param>
        public void ShowTips(string tips,int itemId = 0)
        {
            GameGod.Instance.UIManager.OpenUI<UITips>(E_UILevel.Tips, tips, itemId);
        }
    }
}