/*********************************************
 * BFramework
 * 表格扩展类
 * 修改时间：2022/09/18 08:09:31
 *********************************************/
using Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameData
{
    /// <summary>
    /// 全局表
    /// </summary>
    public partial class TableGlobal
    {
        public int[] _mapMaxArr;
        /// <summary>
        /// 地图最大值 层，行，列
        /// </summary>
        public int[] MapMaxArr
        {
            get
            {
                if(_mapMaxArr== null)
                {
                    _mapMaxArr = Map_Max.SplitToIntArr(',');
                }
                return _mapMaxArr;
            }
        }
    }

    /// <summary>
    /// 道具表
    /// </summary>
    //public partial class TableProp
    //{
    //    /// <summary>
    //    /// 获得道具图片
    //    /// </summary>
    //    public Sprite GetSpIcon(LoadHelper loadHelper) => loadHelper.GetSprite("",Asset);
    //}

    /// <summary>
    /// 卡牌表
    /// </summary>
    public partial class TableSuitCard
    {
        /// <summary>
        /// 获得卡牌图片
        /// </summary>
        public Sprite GetSpCard(LoadHelper loadHelper) => loadHelper.GetSprite(AtlasName.UI, Asset);
    }

    /// <summary>
    /// 关卡表
    /// </summary>
    public partial class TableLevel
    {
        private List<int[]> _monsterInfoList;
        /// <summary>
        /// 刷怪信息列表
        /// </summary>
        public List<int[]> MonsterInfoList
        {
            get
            {
                if (_monsterInfoList == null)
                {
                    _monsterInfoList = new List<int[]>();
                    var strArr = Array_Monster_Info.Split('|');
                    for (int i = 0,length = strArr.Length; i < length; i++)
                    {
                        _monsterInfoList.Add(strArr[i].SplitToIntArr(','));
                    }
                }
                return _monsterInfoList;
            }
        }
    }
}
