/*********************************************
 * 自动生成代码，禁止手动修改文件
 * 脚本名：Main_CardUnit.cs
 * 修改时间：2023/04/24 10:46:58
 *********************************************/

using Framework;
using UnityEngine;
using UnityEngine.UI;

namespace GameData
{
    public partial class Main_CardUnit
    {
        /// <summary>
        /// 
        /// </summary>
        public Framework.ImageEx Img_Card;

        /// <summary>
        /// 
        /// </summary>
        public Framework.TextEx Txt_Name;

        public override void OnCreate()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            Img_Card = rectTransform.Find("Img_Card").GetComponent<Framework.ImageEx>();
			Txt_Name = rectTransform.Find("Txt_Name").GetComponent<Framework.TextEx>();
			
        }
    }
}
