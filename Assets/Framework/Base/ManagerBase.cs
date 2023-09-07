/*********************************************
 * BFramework
 * 管理类基类
 * 创建时间：2023/01/08/ 20:40:23
 *********************************************/
using MainPackage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 管理类基类
    /// </summary>
    public abstract class ManagerBase
    {
        public ManagerBase()
        {
            GameGod.Instance.Log(E_Log.Framework, "初始化管理器", GetType().Name);
            OnInit();
        }

        /// <summary>
        /// 初始化Manager
        /// </summary>
        public abstract void OnInit();
        /// <summary>
        /// Update
        /// </summary>
        public abstract void OnUpdate();
        /// <summary>
        /// 关闭Manager
        /// </summary>
        public abstract void OnDispose();
    }
}