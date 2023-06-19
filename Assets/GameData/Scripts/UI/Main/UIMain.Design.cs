/*********************************************
 * 自动生成代码，禁止手动修改文件
 * 脚本名：UIMain.Design.cs
 * 修改时间：2023/04/24 09:57:45
 *********************************************/

using Framework;
using UnityEngine;
using UnityEngine.UI;

namespace GameData
{
    public partial class UIMain
    {
        /// <summary>
        /// 
        /// </summary>
        public UnityEngine.RectTransform Rt_CardUnitRoot;

        public override void OnCreate()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            Rt_CardUnitRoot = rectTransform.Find("Rt_CardUnitRoot").GetComponent<UnityEngine.RectTransform>();
			
        }
    }
}
