/*********************************************
 * 自动生成代码，禁止手动修改文件
 * 脚本名：UITips.Design.cs
 * 修改时间：2024/07/28 19:25:47
 *********************************************/

using Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameData
{
    public partial class UITips
    {
        public GameObject Unit_Tips;

        public UnitPool<Unit_Tips> Unit_TipsPool;

        public override void OnCreate()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            Unit_Tips = rectTransform.Find("Unit_Tips").gameObject;
			Unit_TipsPool = new UnitPool<Unit_Tips>(this,Unit_Tips);
			
        }
    }
}
