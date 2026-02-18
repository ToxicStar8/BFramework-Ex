/*********************************************
 * 
 * 脚本名：Unit_Tips.cs
 * 创建时间：2026/02/19 02:31:45
 *********************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

namespace GameData
{
    public partial class Unit_Tips : UnitBase
    {
        public override void OnAwake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void OnShow(/*TableProp tbProp,*/ string tips)
        {
            Tmp_Tips.text = tips;
        }
        
        public override void OnRecycle()
        {
            
        }

        public override void OnBeforeDestroy()
        {
            
        }
    }
}
