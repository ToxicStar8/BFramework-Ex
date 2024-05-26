/*********************************************
 * 
 * 脚本名：UIMainMenu.cs
 * 创建时间：2023/03/14 11:19:54
 *********************************************/
using Framework;
using MainPackage;
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
            if (Input.GetKeyDown(KeyCode.A))
            {
                ShowTips("测试1");

                //AddTimer("1", TimerInfo.Create(4, 1000, true, () =>
                //{
                //    Log(E_Log.Log, Time.time.ToString());
                //}));
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                //RemoveTimer("1");
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                //for (int i = 0; i < 5; i++)
                //{
                //    AddTimer(i.ToString(), TimerInfo.Create(4, 1000, true, () =>
                //    {
                //        Log(E_Log.Log, Time.time.ToString());
                //    }));
                //}
            }
        }

        public override void OnShow(params object[] args)
        {

        }

        private void OnClick_Btn_Start()
        {
            Log(E_Log.Log, "开始游戏");
        }

        private void OnClick_Btn_Continue()
        {
            Log(E_Log.Log, "继续游戏");
        }

        private void OnClick_Btn_Exit()
        {
            Log(E_Log.Log, "退出游戏");
            Application.Quit();
        }

        protected override void OnBeforeDestroy()
        {

        }
    }
}
