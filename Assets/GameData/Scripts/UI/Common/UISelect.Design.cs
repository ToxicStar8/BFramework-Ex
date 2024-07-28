/*********************************************
 * 自动生成代码，禁止手动修改文件
 * 脚本名：UISelect.Design.cs
 * 修改时间：2024/07/28 19:33:13
 *********************************************/

using Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameData
{
    public partial class UISelect
    {
        public ButtonEx Btn_Mask;

        public TextMeshProUGUI Tmp_Title;

        public RectTransform Rt_Group;

        public ButtonEx Btn_A;

        public TextMeshProUGUI Tmp_A;

        public ButtonEx Btn_B;

        public TextMeshProUGUI Tmp_B;

        public ButtonEx Btn_C;

        public TextMeshProUGUI Tmp_C;

        public ButtonEx Btn_D;

        public TextMeshProUGUI Tmp_D;

        public override void OnCreate()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            Btn_Mask = rectTransform.Find("Btn_Mask").GetComponent<ButtonEx>();
			Tmp_Title = rectTransform.Find("Tmp_Title").GetComponent<TextMeshProUGUI>();
			Rt_Group = rectTransform.Find("Rt_Group").GetComponent<RectTransform>();
			Btn_A = rectTransform.Find("Rt_Group/Btn_A").GetComponent<ButtonEx>();
			Tmp_A = rectTransform.Find("Rt_Group/Btn_A/Tmp_A").GetComponent<TextMeshProUGUI>();
			Btn_B = rectTransform.Find("Rt_Group/Btn_B").GetComponent<ButtonEx>();
			Tmp_B = rectTransform.Find("Rt_Group/Btn_B/Tmp_B").GetComponent<TextMeshProUGUI>();
			Btn_C = rectTransform.Find("Rt_Group/Btn_C").GetComponent<ButtonEx>();
			Tmp_C = rectTransform.Find("Rt_Group/Btn_C/Tmp_C").GetComponent<TextMeshProUGUI>();
			Btn_D = rectTransform.Find("Rt_Group/Btn_D").GetComponent<ButtonEx>();
			Tmp_D = rectTransform.Find("Rt_Group/Btn_D/Tmp_D").GetComponent<TextMeshProUGUI>();
			
        }
    }
}
