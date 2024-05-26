/*********************************************
 * 自动生成代码，禁止手动修改文件
 * 脚本名：Unit_Tips.cs
 * 修改时间：2024/05/26 17:14:26
 *********************************************/

using Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameData
{
    public partial class Unit_Tips
    {
        public TextEx Txt_Tips;

        public ImageEx Img_Item;

        public override void OnCreate()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            Txt_Tips = rectTransform.Find("Txt_Tips").GetComponent<TextEx>();
			Img_Item = rectTransform.Find("Txt_Tips/Img_Item").GetComponent<ImageEx>();
			
        }
    }
}
