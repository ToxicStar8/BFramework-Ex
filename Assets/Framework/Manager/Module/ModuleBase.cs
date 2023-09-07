/*********************************************
 * BFramework
 * 脚本名：ModuleBase.cs
 * 创建时间：2023/04/06 11:45:09
 *********************************************/
using GameData;
using System;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 数据操作基类
    /// </summary>
    public abstract class ModuleBase : GameBase
    {
        private EmtpyMono _emtpyMono;
        protected EmtpyMono EmtpyMono
        {
            get
            {
                _emtpyMono = _emtpyMono ?? new EmtpyMono();
                return _emtpyMono;
            }
        }

        /// <summary>
        /// 注册监听
        /// </summary>
        public abstract void OnInit();

        #region Event
        private List<ushort> _eventList;
        public void AddEventListener(ModuleEvent eventNo, Action<object[]> callBack)
        {
            AddEventListener((ushort)eventNo, callBack);
        }
        public override void AddEventListener(ushort eventNo, Action<object[]> callBack)
        {
            if (_eventList == null)
            {
                _eventList = new List<ushort>();
            }
            _eventList.Add(eventNo);
            base.AddEventListener(eventNo, callBack);
        }
        #endregion

        /// <summary>
        /// 关闭Module
        /// </summary>
        public virtual void OnDispose()
        {
            //关闭前移除全部注册事件
            if (_eventList != null)
            {
                for (int i = 0, count = _eventList.Count; i < count; i++)
                {
                    RemoveEventListener(_eventList[i]);
                }
                _eventList.Clear();
                _eventList = null;
            }
        }
    }
}
