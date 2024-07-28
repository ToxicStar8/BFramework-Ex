/*********************************************
 * 自动生成代码，禁止手动修改文件
 * 脚本名：UIPause.Design.cs
 * 修改时间：2024/07/28 19:41:48
 *********************************************/

using Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameData
{
    public partial class UIPause
    {
        public ImageEx Img_Mask;

        public ImageEx Img_Bg;

        public TextMeshProUGUI Tmp_Content;

        public ButtonEx Btn_Back;

        public override void OnCreate()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            Img_Mask = rectTransform.Find("Img_Mask").GetComponent<ImageEx>();
			Img_Bg = rectTransform.Find("Img_Bg").GetComponent<ImageEx>();
			Tmp_Content = rectTransform.Find("Tmp_Content").GetComponent<TextMeshProUGUI>();
			Btn_Back = rectTransform.Find("Btn_Back").GetComponent<ButtonEx>();
			
        }
    }
}
