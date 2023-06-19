/*********************************************
 * 自动生成代码，禁止手动修改文件
 * 脚本名：UIMainMenu.Design.cs
 * 修改时间：2023/06/19 14:16:06
 *********************************************/

using Framework;
using UnityEngine;
using UnityEngine.UI;

namespace GameData
{
    public partial class UIMainMenu
    {
        /// <summary>
        /// 
        /// </summary>
        public UnityEngine.UI.Button Btn_Start;

        /// <summary>
        /// 
        /// </summary>
        public UnityEngine.UI.Button Btn_Continue;

        /// <summary>
        /// 
        /// </summary>
        public UnityEngine.UI.Button Btn_Exit;

        public override void OnCreate()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            Btn_Start = rectTransform.Find("Btn_Start").GetComponent<UnityEngine.UI.Button>();
			Btn_Continue = rectTransform.Find("Btn_Continue").GetComponent<UnityEngine.UI.Button>();
			Btn_Exit = rectTransform.Find("Btn_Exit").GetComponent<UnityEngine.UI.Button>();
			
        }
    }
}
