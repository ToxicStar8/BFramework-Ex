/*********************************************
 * 自动生成代码，禁止手动修改文件
 * 脚本名：UIChange.Design.cs
 * 修改时间：2024/04/21 21:38:12
 *********************************************/

using Framework;
using UnityEngine;
using UnityEngine.UI;

namespace GameData
{
    public partial class UIMask
    {
        /// <summary>
        /// 
        /// </summary>
        public Framework.ImageEx Img_Mask;

        public override void OnCreate()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            Img_Mask = rectTransform.Find("Img_Mask").GetComponent<Framework.ImageEx>();
			
        }
    }
}
