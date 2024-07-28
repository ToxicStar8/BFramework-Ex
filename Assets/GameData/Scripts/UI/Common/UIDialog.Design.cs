/*********************************************
 * 自动生成代码，禁止手动修改文件
 * 脚本名：UIDialog.Design.cs
 * 修改时间：2024/07/28 19:23:33
 *********************************************/

using Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameData
{
    public partial class UIDialog
    {
        public ButtonEx Btn_Mask;

        public ImageEx Img_Bg;

        public TextMeshProUGUI Tmp_Content;

        public TextMeshProUGUI Tmp_Title;

        public ButtonEx Btn_Cancel;

        public TextMeshProUGUI Tmp_Cancel;

        public ButtonEx Btn_Confirm;

        public TextMeshProUGUI Tmp_Confirm;

        public override void OnCreate()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            Btn_Mask = rectTransform.Find("Btn_Mask").GetComponent<ButtonEx>();
			Img_Bg = rectTransform.Find("Img_Bg").GetComponent<ImageEx>();
			Tmp_Content = rectTransform.Find("Tmp_Content").GetComponent<TextMeshProUGUI>();
			Tmp_Title = rectTransform.Find("Tmp_Content/Tmp_Title").GetComponent<TextMeshProUGUI>();
			Btn_Cancel = rectTransform.Find("Tmp_Content/Btns/Btn_Cancel").GetComponent<ButtonEx>();
			Tmp_Cancel = rectTransform.Find("Tmp_Content/Btns/Btn_Cancel/Tmp_Cancel").GetComponent<TextMeshProUGUI>();
			Btn_Confirm = rectTransform.Find("Tmp_Content/Btns/Btn_Confirm").GetComponent<ButtonEx>();
			Tmp_Confirm = rectTransform.Find("Tmp_Content/Btns/Btn_Confirm/Tmp_Confirm").GetComponent<TextMeshProUGUI>();
			
        }
    }
}
