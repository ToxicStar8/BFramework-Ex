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

        //按钮按下拿起回调
        public Action<PointerEventData> OnPointerDownCallback;
        public Action<PointerEventData> OnPointerUpCallback;

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            OnPointerDownCallback?.Invoke(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            OnPointerUpCallback?.Invoke(eventData);
        }
    }
}
