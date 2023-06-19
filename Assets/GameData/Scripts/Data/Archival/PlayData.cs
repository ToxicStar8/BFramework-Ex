/*********************************************
 * 
 * 玩家数据
 * 创建时间：2023/01/29 15:30:23
 *********************************************/
using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameData
{
    /// <summary>
    /// 玩家数据
    /// </summary>
    public class PlayData : DataBase
    {
        public override void OnInit()
        {
            //初始化角色数据
            RoleData = new RoleData();
            //初始化持有卡牌列表
            CardHeldList = new List<CardData>();
            var tbSuitCardCtrl = ModuleBase.TbSuitCardCtrl;
            for (int i = 0,count = tbSuitCardCtrl.Count; i < count; i++)
            {
                var tbSuitCard = tbSuitCardCtrl[i];
                if(tbSuitCard.Suit_Type != (int)E_SuitType.Joker)
                {
                    CardHeldList.Add(new CardData()
                    {
                        CfgId = tbSuitCard.DataId,
                        Count = 1,
                    });
                }
            }
            CardHeldList.SortByDisordered();
        }
        public RoleData RoleData { private set; get; }
        public TableRole GetTbRole() => RoleData.GetTbRole();
        /// <summary>
        /// 持有的卡牌列表
        /// </summary>
        public List<CardData> CardHeldList { private set; get; }
        //当前卡牌的位置
        public int CurSelectIdx;
        public CardData GetCurCardData() => CardHeldList[CurSelectIdx];
        public override void OnDispose()
        {

        }
    }
}
