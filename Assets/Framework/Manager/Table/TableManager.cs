/*********************************************
 * BFramework
 * 表管理器
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using MainPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 表管理器
    /// </summary>
    public class TableManager : ManagerBase
    {
        private Dictionary<Type, ITableCtrlBase> _allTableDic;

        public override void OnInit()
        {
            _allTableDic = new Dictionary<Type, ITableCtrlBase>();
        }

        public void Init(Type[] typeArr)
        {
            //初始化表格
            for (int i = 0, length = typeArr.Length; i < length; i++)
            {
                var tableType = typeArr[i];
                var tableCtrl = Activator.CreateInstance(tableType) as ITableCtrlBase;
                _allTableDic.Add(tableType, tableCtrl);
            }
        }

        /// <summary>
        /// 获取表格控制器
        /// </summary>
        public T GetTableCtrl<T>() where T : class, ITableCtrlBase
        {
            Type type = typeof(T);
            if (!_allTableDic.TryGetValue(type, out var tableCtrl))
            {
                GameGod.Instance.Log(E_Log.Error, type.Name, "未进行初始化！");
                return null;
            }

            //用的时候再更新数据
            if (tableCtrl.GetCreateStatus() == 0)
            {
                GameGod.Instance.Log(E_Log.Framework, "初始化表格", type.Name);
                tableCtrl.OnInit();
            }
            return _allTableDic[type] as T;
        }

        /// <summary>
        /// 热重载
        /// </summary>
        public void HotReload()
        {
            foreach (var item in _allTableDic)
            {
                var tableCtrl = item.Value;
                if (tableCtrl.GetCreateStatus() == 2)
                {
                    GameGod.Instance.Log(E_Log.Framework, "热重载表格", tableCtrl.GetType().Name);
                    tableCtrl.OnInit();
                }
            }
        }

        public override void OnUpdate() { }
        public override void OnDispose()
        {
            _allTableDic.Clear();
            _allTableDic = null;
        }
    }
}