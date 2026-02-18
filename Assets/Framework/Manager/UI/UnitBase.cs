/*********************************************
 * BFramework
 * PanelUnit基类
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// PanelUnit基类
    /// </summary>
    public abstract class UnitBase : GameBaseMono
    {
        /// <summary>
        /// Unit游戏节点
        /// </summary>
        [HideInInspector]
        public RectTransform rectTransform;

        /// <summary>
        /// 加载器
        /// </summary>
        public LoadHelper LoadHelper;

        public abstract void OnAwake();
        public abstract void OnRecycle();
        public abstract void OnBeforeDestroy();
    }
}