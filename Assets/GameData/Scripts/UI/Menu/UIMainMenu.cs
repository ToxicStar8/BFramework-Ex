/*********************************************
 * 
 * 脚本名：UIMainMenu.cs
 * 创建时间：2023/03/14 11:19:54
 *********************************************/
using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameData
{
    public partial class UIMainMenu : GameUIBase
    {
        public override void OnInit()
        {
            Btn_Start.AddListener(OnClick_Btn_Start);
            Btn_Continue.AddListener(OnClick_Btn_Continue);
            Btn_Exit.AddListener(OnClick_Btn_Exit);

            RegisterUpdate(Update);
        }

        private void Update()
        {

        }

        public override void OnShow(params object[] args)
        {
            Btn_Continue.SetActive(!GameGod.Instance.DataManager.IsNullData);
        }

        private void FnContinueGame()
        {
            GameManager.Instance.InitGame();
            CloseSelf();
        }

        private void OnClick_Btn_Start()
        {
            //PlayerModule.FnStartNewGame(new Action(() =>
            //{
                FnContinueGame();
            //}));
        }

        private void OnClick_Btn_Continue()
        {
            FnContinueGame();
        }

        private void OnClick_Btn_Exit()
        {
            Application.Quit();
        }

        public override void OnBeforDestroy()
        {

        }
    }
}
