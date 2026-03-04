/*********************************************
 * BFramework
 * UI基类
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using MainPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// UI基类
    /// </summary>
    public abstract class UIBase : GameBaseMono
    {
        /// <summary>
        /// UI游戏节点
        /// </summary>
        [HideInInspector]
        public RectTransform rectTransform;

        /// <summary>
        /// UI名字
        /// </summary>
        [HideInInspector]
        public string UIName;

        /// <summary>
        /// 加载器
        /// </summary>
        public LoadHelper LoadHelper;

        /// <summary>
        /// 协程
        /// </summary>
        protected Coroutine Coroutine;

        /// <summary>
        /// 自动清理器
        /// </summary>
        protected readonly AutoCleanup Cleanup = new();

        /// <summary>
        /// 获取UI节点
        /// </summary>
        protected RectTransform GetUILevelTrans(E_UILevel uiLevel)
        {
            return GameManager.Instance.GetUILevelTrans(uiLevel);
        }

        /// <summary>
        /// 关闭自己
        /// </summary>
        public void CloseSelf()
        {
            CloseUI(UIName);
        }

        /// <summary>
        /// 隐藏自己
        /// </summary>
        public void HideSelf()
        {
            HideUI(UIName);
        }

        #region Update
        protected void RegisterUpdate(Action updateCallback)
        {
            Cleanup.RegisterUpdate(gameObject.name, updateCallback);
        }
        #endregion

        #region Event
        protected override void AddEventListener(uint eventNo, Action<object[]> callBack)
        {
            Cleanup.TrackEvent(eventNo, callBack);
            base.AddEventListener(eventNo, callBack);
        }
        #endregion

        #region Timer
        /// <summary>
        /// 添加定时器监听
        /// </summary>
        protected override void AddTimer(string timeName, TimerInfo timerInfo)
        {
            Cleanup.TrackTimer(timeName);
            base.AddTimer(timeName, timerInfo);
        }

        protected override void AddTempTimer(TimerInfo timerInfo)
        {
            Cleanup.TrackTimer(timerInfo.TimerName);
            base.AddTempTimer(timerInfo);
        }
        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        public abstract void OnAwake();

        /// <summary>
        /// 每次打开时调用
        /// </summary>
        public abstract void OnShow(params object[] args);

        /// <summary>
        /// 销毁前调用
        /// </summary>
        protected abstract void OnBeforeDestroy();

        /// <summary>
        /// 关闭UI通用方法
        /// </summary>
        public void OnDispose()
        {
            //关闭前执行
            OnBeforeDestroy();

            //回收加载器
            LoadHelper.Recycle(LoadHelper);

            //统一清理事件、Update、定时器
            Cleanup.Dispose();

            //移除协程
            if (Coroutine != null)
            {
                GameManager.Instance.StopCoroutine(Coroutine);
            }
        }
    }
}