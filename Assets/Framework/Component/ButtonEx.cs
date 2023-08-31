/*********************************************
 * BFramework
 * 按钮扩展类
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework
{
    /// <summary>
    /// 按钮扩展类
    /// </summary>
    public class ButtonEx : Button
    {
        private RectTransform selfRect;
        /// <summary>
        /// Btn游戏节点
        /// </summary>
        public RectTransform rectTransform
        {
            get
            {
                if (selfRect == null)
                {
                    selfRect = GetComponent<RectTransform>();
                }
                return selfRect;
            }
        }

        //按钮进入、按下、点击、拿起、离开回调
        public Action<PointerEventData> OnPointerEnterCallback;
        public Action<PointerEventData> OnPointerDownCallback;
        public Action<PointerEventData> OnPointerClickCallback;
        public Action<PointerEventData> OnPointerUpCallback;
        public Action<PointerEventData> OnPointerExitCallback;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            OnPointerEnterCallback?.Invoke(eventData);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            OnPointerDownCallback?.Invoke(eventData);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            OnPointerClickCallback?.Invoke(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            OnPointerUpCallback?.Invoke(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            OnPointerExitCallback?.Invoke(eventData);   
        }
    }
}
