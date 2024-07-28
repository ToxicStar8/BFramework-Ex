/*********************************************
 * 自动生成代码，禁止手动修改文件
 * 脚本名：UIAddItemTips.Design.cs
 * 修改时间：2024/07/28 19:27:50
 *********************************************/

using Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameData
{
    public partial class UIAddItemTips
    {
        public GameObject Unit_AddItemTips;

        public UnitPool<Unit_AddItemTips> Unit_AddItemTipsPool;

        public override void OnCreate()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            Unit_AddItemTips = rectTransform.Find("Unit_AddItemTips").gameObject;
			Unit_AddItemTipsPool = new UnitPool<Unit_AddItemTips>(this,Unit_AddItemTips);
			
        }
    }
}
