/*********************************************
 * 自动生成代码，禁止手动修改文件
 * 脚本名：Unit_AddItemTips.cs
 * 修改时间：2024/07/28 19:27:50
 *********************************************/

using Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameData
{
    public partial class Unit_AddItemTips
    {
        public TextMeshProUGUI Tmp_Tips;

        public ImageEx Img_Item;

        public override void OnCreate()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            Tmp_Tips = rectTransform.Find("Tmp_Tips").GetComponent<TextMeshProUGUI>();
			Img_Item = rectTransform.Find("Tmp_Tips/Img_Item").GetComponent<ImageEx>();
			
        }
    }
}
