/*********************************************
 * 自动生成代码，禁止手动修改文件
 * 脚本名：UIMainMenu.Design.cs
 * 修改时间：2024/07/27 23:15:18
 *********************************************/

using Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameData
{
    public partial class UIMainMenu
    {
        public ButtonEx Btn_Start;

        public TextMeshProUGUI Tmp_Start;

        public ButtonEx Btn_Continue;

        public TextMeshProUGUI Tmp_Continue;

        public ButtonEx Btn_Exit;

        public TextMeshProUGUI Tmp_Exit;

        public TextMeshProUGUI Tmp_Title;

        public override void OnCreate()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            Btn_Start = rectTransform.Find("Btn_Start").GetComponent<ButtonEx>();
			Tmp_Start = rectTransform.Find("Btn_Start/Tmp_Start").GetComponent<TextMeshProUGUI>();
			Btn_Continue = rectTransform.Find("Btn_Continue").GetComponent<ButtonEx>();
			Tmp_Continue = rectTransform.Find("Btn_Continue/Tmp_Continue").GetComponent<TextMeshProUGUI>();
			Btn_Exit = rectTransform.Find("Btn_Exit").GetComponent<ButtonEx>();
			Tmp_Exit = rectTransform.Find("Btn_Exit/Tmp_Exit").GetComponent<TextMeshProUGUI>();
			Tmp_Title = rectTransform.Find("Tmp_Title").GetComponent<TextMeshProUGUI>();
			
        }
    }
}
