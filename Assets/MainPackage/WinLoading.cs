///*********************************************
// * BFramework
// * 加载窗口代码
// * 创建时间：2022/12/28 20:40:23
// *********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MainPackage
{
    public class WinLoading : MonoBehaviour
    {
        /// <summary>
        /// 进度条
        /// </summary>
        [SerializeField]
        private Image Img_Progress;
        /// <summary>
        /// 检查更新中
        /// </summary>
        [SerializeField]
        private Text Txt_Version;
        /// <summary>
        /// 一级提示
        /// </summary>
        [SerializeField]
        private Text Txt_OneTips;
        /// <summary>
        /// 二级提示
        /// </summary>
        [SerializeField]
        private Text Txt_TwoTips;
        /// <summary>
        /// 退出按钮
        /// </summary>
        [SerializeField]
        private Button Btn_Quit;
        /// <summary>
        /// 加载报错
        /// </summary>
        [SerializeField]
        private RectTransform Rt_Error;

        private void Awake()
        {
            Btn_Quit.onClick.AddListener(Application.Quit);
            SetIsComplete(false);
            SetProgress(0);
            SetVersion("");
            SetOneTips("");
            SetTwoTips("");
        }

        /// <summary>
        /// 设置版本号
        /// </summary>
        public void SetVersion(string versionCheck)
        {
            Txt_Version.text = versionCheck;
        }

        /// <summary>
        /// 设置一级提示
        /// </summary>
        public void SetProgress(float progress)
        {
            Img_Progress.fillAmount = progress;
        }

        /// <summary>
        /// 设置一级提示
        /// </summary>
        public void SetOneTips(string oneTips)
        {
            Txt_OneTips.text = oneTips;
        }

        /// <summary>
        /// 设置二级提示
        /// </summary>
        public void SetTwoTips(string twoTips)
        {
            Txt_TwoTips.text = twoTips;
        }

        /// <summary>
        /// 显示异常
        /// </summary>
        public void ShowError()
        {
            Rt_Error.gameObject.SetActive(true);
        }

        /// <summary>
        /// 设置下载完毕
        /// </summary>
        public void SetIsComplete(bool isComplete)
        {
            gameObject.SetActive(!isComplete);
        }
    }
}