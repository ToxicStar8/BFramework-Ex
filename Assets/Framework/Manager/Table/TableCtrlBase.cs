﻿/*********************************************
 * BFramework
 * 表控制器基类
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using MainPackage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 表控制器基类
    /// </summary>
    public abstract class TableCtrlBase<T> : GameBase, ITableCtrlBase where T : TableBase, new()        // 先接口 再限定T，如果先限定T再写接口会判定这是T的接口
    {
        /// <summary>
        /// 表名
        /// </summary>
        public abstract string TableName { get; }

        /// <summary>
        /// 初始化数据状态 0=未初始化 1=初始化中 2=初始化完毕
        /// </summary>
        private int _initDataStatus = 0;

        /// <summary>
        /// 数据列表
        /// </summary>
        public List<T> DataList { private set; get; } = new List<T>();
        public Dictionary<int, T> DataDic { private set; get; } = new Dictionary<int, T>();
        public int Count => DataList.Count;
        public T this[int index] => GetDataByIndex(index);

        /// <summary>
        /// 安全的通过索引获得数据（列表获取）
        /// </summary>
        public T GetDataByIndex(int index)
        {
            if (index > DataList.Count - 1)
            {
                GameGod.Instance.Log(E_Log.Error, "超出数据上限 索引", index.ToString());
                return null;
            }
            return DataList[index];
        }

        /// <summary>
        /// 通过Id获得数据（字典获取）
        /// </summary>
        public T GetDataById(int id)
        {
            if (!DataDic.TryGetValue(id, out var table))
            {
                GameGod.Instance.Log(E_Log.Error, "没有找到表数据 id", id.ToString());
            }
            return table;
        }

        public int GetCreateStatus() => _initDataStatus;

        public void OnInit()
        {
            _initDataStatus = 1;

            //表格的AB包不需要卸载
            var textAsset = GameGod.Instance.LoadManager.LoadSync<TextAsset>(TableName);
            var allArr = textAsset.text.Split("`", System.StringSplitOptions.RemoveEmptyEntries);  //全部的文本
            var nameGroupArr = allArr[0].Split('^');   //变量名的行
                                                       //从第二行开始出数据
            for (int i = 1, length = allArr.Length; i < length; i++)
            {
                T table = new T();
                table.OnInit(nameGroupArr, allArr[i]);
                DataList.Add(table);
                DataDic.Add(table.Id, table);
            }

            _initDataStatus = 2;
        }
    }
}