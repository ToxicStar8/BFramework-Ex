/*********************************************
 * BFramework
 * 自动清理器 - UIBase和ModuleBase共用的事件/更新/定时器跟踪与清理
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using MainPackage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 自动清理器
    /// 跟踪注册的事件、Update回调、定时器，统一在Dispose时移除
    /// </summary>
    public class AutoCleanup
    {
        //记录注册的事件，关闭时自动移除
        private Dictionary<uint, Action<object[]>> _eventList;
        private Action _update;
        private List<string> _timerList;

        #region Event
        /// <summary>
        /// 跟踪事件注册（实际注册仍由调用者通过base.AddEventListener完成）
        /// </summary>
        public void TrackEvent(uint eventNo, Action<object[]> callBack)
        {
            _eventList ??= new();
            _eventList.Add(eventNo, callBack);
        }
        #endregion

        #region Update
        /// <summary>
        /// 注册Update回调并跟踪
        /// </summary>
        public bool RegisterUpdate(string ownerName, Action updateCallback)
        {
            if (_update != null)
            {
                GameManager.Instance.Log(E_Log.Error, ownerName, "Update重复注册！");
                return false;
            }

            _update = updateCallback;
            GameManager.Instance.UpdateCallback += _update;
            return true;
        }
        #endregion

        #region Timer
        /// <summary>
        /// 跟踪定时器注册（实际注册仍由调用者通过base.AddTimer完成）
        /// </summary>
        public void TrackTimer(string timeName)
        {
            _timerList ??= new List<string>();
            _timerList.Add(timeName);
        }
        #endregion

        /// <summary>
        /// 统一清理全部跟踪的事件、Update回调、定时器
        /// </summary>
        public void Dispose()
        {
            //关闭前移除全部Update回调
            if (_update != null)
            {
                GameManager.Instance.UpdateCallback -= _update;
                _update = null;
            }

            //关闭前移除全部注册事件
            if (_eventList != null)
            {
                foreach (var item in _eventList)
                {
                    GameManager.Instance.EventManager.RemoveEventListener(item.Key, item.Value);
                }
                _eventList.Clear();
                _eventList = null;
            }

            //关闭前移除全部定时器
            if (_timerList != null)
            {
                for (int i = 0, count = _timerList.Count; i < count; i++)
                {
                    GameManager.Instance.TimerManager.RemoveTimer(_timerList[i]);
                }
                _timerList.Clear();
                _timerList = null;
            }
        }
    }
}
