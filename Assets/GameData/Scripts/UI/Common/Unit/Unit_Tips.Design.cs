/*********************************************
 * 自动生成代码，禁止手动修改文件
 * 脚本名：Unit_Tips.cs
 * 修改时间：2024/07/28 19:25:47
 *********************************************/

using Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameData
{
    public partial class Unit_Tips
    {
        public TextMeshProUGUI Tmp_Tips;

        public override void OnCreate()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            Tmp_Tips = rectTransform.Find("Tmp_Tips").GetComponent<TextMeshProUGUI>();
			
        }
    }
}
