/*********************************************
 * 自动生成代码，禁止手动修改文件
 * 脚本名：UIMainMenu.Design.cs
 * 修改时间：2024/04/18 22:21:50
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
        public TMPro.TextMeshProUGUI Tmp_Start;

        /// <summary>
        /// 
        /// </summary>
        public UnityEngine.UI.Button Btn_Continue;

        /// <summary>
        /// 
        /// </summary>
        public TMPro.TextMeshProUGUI Tmp_Continue;

        /// <summary>
        /// 
        /// </summary>
        public UnityEngine.UI.Button Btn_Exit;

        /// <summary>
        /// 
        /// </summary>
        public TMPro.TextMeshProUGUI Tmp_Exit;

        public override void OnCreate()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            Btn_Start = rectTransform.Find("Btn_Start").GetComponent<UnityEngine.UI.Button>();
			Tmp_Start = rectTransform.Find("Btn_Start/Tmp_Start").GetComponent<TMPro.TextMeshProUGUI>();
			Btn_Continue = rectTransform.Find("Btn_Continue").GetComponent<UnityEngine.UI.Button>();
			Tmp_Continue = rectTransform.Find("Btn_Continue/Tmp_Continue").GetComponent<TMPro.TextMeshProUGUI>();
			Btn_Exit = rectTransform.Find("Btn_Exit").GetComponent<UnityEngine.UI.Button>();
			Tmp_Exit = rectTransform.Find("Btn_Exit/Tmp_Exit").GetComponent<TMPro.TextMeshProUGUI>();
			
        }
    }
}
