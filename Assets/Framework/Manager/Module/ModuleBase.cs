/*********************************************
 * BFramework
 * 脚本名：ModuleBase.cs
 * 创建时间：2023/04/06 11:45:09
 *********************************************/

using MainPackage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 数据操作基类
    /// </summary>
    public abstract class ModuleBase : GameBase
    {
        /// <summary>
        /// 自动清理器
        /// </summary>
        protected readonly AutoCleanup Cleanup = new();

        public ModuleBase()
        {
            Log(E_Log.Framework, "初始化Module", this.GetType().Name);
        }

        /// <summary>
        /// 初始化Module，只有New时会执行，Load时不执行
        /// </summary>
        public abstract void OnNew();

        /// <summary>
        /// Load存档时执行，只有Load时会执行，New时不执行
        /// </summary>
        public abstract void OnLoad();

        #region Event
        protected override void AddEventListener(uint eventNo, Action<object[]> callBack)
        {
            Cleanup.TrackEvent(eventNo, callBack);
            base.AddEventListener(eventNo, callBack);
        }
        #endregion

        #region Update
        protected void RegisterUpdate(Action updateCallback)
        {
            Cleanup.RegisterUpdate(GetType().Name, updateCallback);
        }
        #endregion

        /// <summary>
        /// 注销前调用
        /// </summary>
        public abstract void OnBeforeDispose();

        /// <summary>
        /// 关闭Module
        /// </summary>
        public void OnDispose()
        {
            //注销前执行
            OnBeforeDispose();

            //统一清理事件、Update回调
            Cleanup.Dispose();

            Log(E_Log.Framework, "注销Module", this.GetType().Name);
        }
    }
}
