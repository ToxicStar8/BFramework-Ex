/*********************************************
 * 
 * 脚本名：UIMain.cs
 * 创建时间：2023/03/26 22:05:12
 *********************************************/
using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    public partial class UIMain : GameUIBase
    {
        private UnitPool<Main_CardUnit> _cardUnitPool;

        public override void OnInit()
        {
            _cardUnitPool = CreateSinglePool<Main_CardUnit>();

            AddEventListener(UIEvent.OnUIMainShowCard, OnUIMainShowCard);
        }

        private void OnUIMainShowCard(object[] args)
        {
            var cardIdx = (args[0] as string).ToInt();
            _cardUnitPool.RecycleAll();
            var unit = _cardUnitPool.CreateUnit(Rt_CardUnitRoot);
            var data = PlayerModule.PlayData.CardHeldList[cardIdx];
            unit.FnShow(data);
        }

        public override void OnShow(params object[] args)
        {

        }

        public override void OnBeforDestroy()
        {
            
        }
    }
}
