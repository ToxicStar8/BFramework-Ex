/*********************************************
 * 
 * 脚本名：Unit_AddItemTips.cs
 * 创建时间：2026/02/19 02:27:44
 *********************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

namespace GameData
{
    public partial class Unit_AddItemTips : UnitBase
    {
        public override void OnAwake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void OnShow(/*TableProp tbProp,*/ int itemCount)
        {
            Tmp_Tips.text = "看配表" + "+" + itemCount.ToString();
        }
        
        public override void OnRecycle()
        {
            
        }

        public override void OnBeforeDestroy()
        {
            
        }
    }
}
