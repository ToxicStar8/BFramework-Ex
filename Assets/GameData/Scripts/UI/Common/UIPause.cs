/*********************************************
 * 
 * 脚本名：UIPause.cs
 * 创建时间：2024/04/21 22:23:31
 *********************************************/
using Framework;
using MainPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    public partial class UIPause : GameUIBase
    {
        public override void OnInit()
        {
            Btn_Back.AddListener(() =>
            {
                Time.timeScale = 1;
#if UNITY_EDITOR
                GameEntry.Instance.TimeScale = 1;
#endif
                CloseSelf();
            });
        }

        public override void OnShow(params object[] args)
        {
            
        }

        public override void OnBeforeDestroy()
        {
            
        }
    }
}
