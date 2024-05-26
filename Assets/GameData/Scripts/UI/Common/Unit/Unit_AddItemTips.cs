/*********************************************
 * 
 * 脚本名：Unit_AddItemTips.cs
 * 创建时间：2024/05/26 17:34:28
 *********************************************/
using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    public partial class Unit_AddItemTips : UnitBase
    {
        public override void OnInit()
        {
            
        }

        public void OnShow(/*TableProp tbProp,*/ int itemCount)
        {
            //Txt_Tips.text = tbProp.Name + "+" + itemCount.ToString();
            Txt_Tips.text = "看配表" + "+" + itemCount.ToString();
        }
    }
}
