/*********************************************
 * 
 * 脚本名：UIMainMenu.cs
 * 创建时间：2026/03/03 18:49:58
 *********************************************/
using Framework;
using MainPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    public partial class UIMainMenu : UIBase
    {
        public override void OnAwake()
        {
            rectTransform = GetComponent<RectTransform>();

            Btn_Start.AddListener(OnClick_Btn_Start);
            Btn_Continue.AddListener(OnClick_Btn_Continue);
            Btn_Exit.AddListener(OnClick_Btn_Exit);
        }

        public override void OnShow(params object[] args)
        {
            
        }

        private void OnClick_Btn_Start()
        {
            Log("点击了Btn_Start");
        }

        private void OnClick_Btn_Continue()
        {
            Log("点击了Btn_Continue");
        }

        private void OnClick_Btn_Exit()
        {
            Log("点击了Btn_Exit");
        }

        protected override void OnBeforeDestroy()
        {
            
        }
    }
}
