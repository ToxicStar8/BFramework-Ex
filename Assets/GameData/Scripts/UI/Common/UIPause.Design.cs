/*********************************************
 * 自动生成代码，禁止手动修改文件
 * 脚本名：UIPause.Design.cs
 * 修改时间：2024/04/21 22:25:01
 *********************************************/

using Framework;
using UnityEngine;
using UnityEngine.UI;

namespace GameData
{
    public partial class UIPause
    {
        /// <summary>
        /// 
        /// </summary>
        public Framework.ImageEx Img_Bg;

        /// <summary>
        /// 
        /// </summary>
        public Framework.ButtonEx Btn_Back;

        public override void OnCreate()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            Img_Bg = rectTransform.Find("Img_Bg").GetComponent<Framework.ImageEx>();
			Btn_Back = rectTransform.Find("Btn_Back").GetComponent<Framework.ButtonEx>();
			
        }
    }
}
