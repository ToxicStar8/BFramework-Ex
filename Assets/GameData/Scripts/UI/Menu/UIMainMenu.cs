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
                AddTempTimer(TimerInfo.Create(101, 100, false, () =>
                {
                    var curTime = TimeUtil.GetNowTimeMilliseconds();
                    Log(E_Log.Log, curTime.ToString());
                }));
            }
            if (Input.GetKeyDown(KeyCode.S))
            {

            }
            if (Input.GetKeyDown(KeyCode.D))
            {

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
