﻿/*********************************************
 * 
 * 脚本名：UIMainMenu.cs
 * 创建时间：2023/03/14 11:19:54
 *********************************************/
using Cysharp.Threading.Tasks;
using Framework;
using MainPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
            var tbRegionCtrl = GetTableCtrl<TableRegionCtrl>();
            for (int i = 0; i < tbRegionCtrl.Count; i++)
            {
                var tbRegion = tbRegionCtrl.DataList[i];
                var sb = new StringBuilder();
                sb.Append(tbRegion.Id);
                sb.Append(" ");
                sb.Append(tbRegion.Region);
                sb.Append(" ");
                //for (int j = 0; j < tbRegion.List_Test1.Count; j++)
                //{
                //    sb.Append(tbRegion.List_Test1[j]);
                //    sb.Append(" ");
                //}

                //for (int j = 0; j < tbRegion.List_Test2.Count; j++)
                //{
                //    sb.Append(tbRegion.List_Test2[j]);
                //    sb.Append(" ");
                //}

                //for (int j = 0; j < tbRegion.List_Test3.Count; j++)
                //{
                //    sb.Append(tbRegion.List_Test3[j]);
                //    sb.Append(" ");
                //}

                //for (int j = 0; j < tbRegion.List_Test4.Count; j++)
                //{
                //    sb.Append(tbRegion.List_Test4[j]);
                //    sb.Append(" ");
                //}

                //for (int j = 0; j < tbRegion.List_Test6.Count; j++)
                //{
                //    var data = tbRegion.List_Test6[j];
                //    for (int k = 0; k < data.Length; k++)
                //    {
                //        sb.Append(data[k]);
                //    }
                //    sb.Append(" ");
                //}

                sb.Append(tbRegion.Test7.x);
                sb.Append(tbRegion.Test7.y);

                sb.Append(" ");

                sb.Append(tbRegion.Test8.x);
                sb.Append(tbRegion.Test8.y);
                sb.Append(tbRegion.Test8.z);

                Log(sb.ToString());
            }

            AddTask(async (task) =>
            {
                Log("哈哈1");
                await UniTask.Delay(1000);
                Log("哈哈2");

                task.OnComplete();
            });

            AddTask(async (task) =>
            {
                Log("哈哈3");

                await UniTask.CompletedTask;

                task.OnComplete();
            });
        }

        private void OnClick_Btn_Start()
        {
            Log(E_Log.Log, "开始游戏");
            GameGod.Instance.ModuleManager.NewAllModule();
            GameGod.Instance.ModuleManager.SaveAllModule();
        }

        private void OnClick_Btn_Continue()
        {
            Log(E_Log.Log, "继续游戏");
            GameGod.Instance.ModuleManager.LoadAllModule();
        }

        private void OnClick_Btn_Exit()
        {
            Application.Quit();
        }

        protected override void OnBeforeDestroy() { }
    }
}
