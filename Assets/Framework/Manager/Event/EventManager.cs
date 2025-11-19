/*********************************************
 * BFramework
 * 事件管理器
 * 创建时间：2023/01/08 20:40:23
 *********************************************/

using MainPackage;
using System;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 事件管理器
    /// </summary>
    public class EventManager : ManagerBase
    {
        private Dictionary<uint, List<Action<object[]>>> _eventDic;

        public override void OnInit() 
        {
            _eventDic = new();
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        public void AddEventListener(uint eventNo, Action<object[]> callBack)
        {
            if (!_eventDic.TryGetValue(eventNo,out var list))
            {
                list = new List<Action<object[]>>();
                _eventDic.Add(eventNo, list);
            }

            if (list.Contains(callBack))
            {
                GameGod.Instance.Log(E_Log.Error, "事件重复监听", eventNo.ToString());
                _eventDic.Remove(eventNo);
            }
            list.Add(callBack);
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        public void RemoveEventListener(uint eventNo, Action<object[]> callBack)
        {
            if (_eventDic is null)
                return;

            if (_eventDic.TryGetValue(eventNo,out var list))
            {
                list.Remove(callBack);
            }
        }

        /// <summary>
        /// 发送事件
        /// </summary>
        public void SendEvent(uint eventNo,params object[] args)
        {
            if (!_eventDic.TryGetValue(eventNo, out var list))
            {
                GameGod.Instance.Log(E_Log.Warning, eventNo.ToString() + "事件不存在");
                return;
            }

            foreach (var callBack in list)
            {
                callBack.Invoke(args);
            }
        }

        public override void OnUpdate() { }
        public override void OnDispose() 
        {
            _eventDic.Clear();
            _eventDic = null;
        }
    }
}
