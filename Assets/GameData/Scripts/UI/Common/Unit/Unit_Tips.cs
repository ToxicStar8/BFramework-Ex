/*********************************************
 * 
 * 脚本名：Unit_Tips.cs
 * 创建时间：2024/07/28 19:18:52
 *********************************************/
using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    public partial class Unit_Tips : UnitBase
    {
        public override void OnInit()
        {
            
        }

        public void OnShow(/*TableProp tbProp,*/ string tips)
        {
            Tmp_Tips.text = tips;
        }
    }
}
