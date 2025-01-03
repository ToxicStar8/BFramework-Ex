/*********************************************
 * BFramework
 * 红点管理器，使用特殊方式实现
 * 创建时间：2025/01/03 21:50:23
 *********************************************/
using MainPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 红点管理器
    /// </summary>
    public class RedPointManager : ManagerBase
    {
        /// <summary>
        /// 红点类型的数量 key为红点类型 value为红点数量 0=没有 n=表示几个
        /// </summary>
        private Dictionary<int, int> _redPointDic { set; get; }

        /// <summary>
        /// 回调的第一个值是Id 第二个值是Key 第三个值是红点的个数 0=不需要显示 1=数量为1 n=数量为n
        /// </summary>
        private Dictionary<int, Action<int, string, int>> _redActions { set; get; }

        public override void OnInit()
        {
            _redPointDic = new();
            _redActions = new();
        }

        public override void OnUpdate()
        {

        }

        /// <summary>
        /// 注册指定类型的红点监听事件
        /// </summary>
        public void AddRedCallBack(int id, Action<int, string, int> callback)
        {
            if (_redActions.ContainsKey(id))
            {
                _redActions[id] += callback;
            }
            else
            {
                _redActions.Add(id, callback);
            }
        }

        /// <summary>
        /// 移除指定类型的红点监听事件
        /// </summary>
        public void RemoveRedCallBack(int id, Action<int, string, int> callback)
        {
            if (_redActions != default && _redActions.ContainsKey(id))
            {
                _redActions[id] -= callback;
                if (_redActions[id] == default)
                {
                    _redActions.Remove(id);
                }
            }
        }

        /// <summary>
        /// 获得指定类型的回调
        /// </summary>
        public Action<int, string, int> GetCallBack(int id)
        {
            _redActions.TryGetValue(id, out Action<int, string, int> value);
            return value;
        }

        /// <summary>
        /// 获取指定类型的红点数量 -1为未初始化 其他表示红点格式
        /// </summary>
        public int GetRedNumById(int id)
        {
            var result = _redPointDic.TryGetValue(id, out int value);
            if (!result)
            {
                return -1;
            }
            return value;
        }

        /// <summary>
        /// 设置指定类型的红点数量
        /// </summary>
        public void SetRedNumById(int id, int num)
        {
            if (_redPointDic.ContainsKey(id))
            {
                _redPointDic[id] = num;
            }
            else
            {
                _redPointDic.TryAdd(id, num);
            }
        }

        // TD 全局设置红点显示

        // TD 全局关闭红点显示

        // TD 全局打开红点显示

        // TD 全局清除所有监听事件

        // TD 全局清除所有的红点数量

        public override void OnDispose()
        {
            _redPointDic.Clear();

            var list = _redActions.Keys.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                _redActions[list[i]] = default;
            }
            _redActions.Clear();
            _redActions = default;
        }
    }
}