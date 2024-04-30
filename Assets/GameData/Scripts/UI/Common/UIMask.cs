/*********************************************
 * 
 * 脚本名：UIChange.cs
 * 创建时间：2024/04/21 21:38:12
 *********************************************/
using DG.Tweening;
using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    public partial class UIMask : GameUIBase
    {
        public override void OnInit()
        {
            AddEventListener(UIEvent.OnCloseUIMask, OnCloseUIMask);
        }

        private void OnCloseUIMask(object[] obj)
        {
            Img_Mask.fillAmount = 1;
            Img_Mask.fillOrigin = 1;
            Img_Mask.DOKill();
            Img_Mask.DOFillAmount(0, 0.2f).onComplete = () =>
            {
                HideSelf();
            };
        }

        public override void OnShow(params object[] args)
        {
            Img_Mask.fillAmount = 0;
            Img_Mask.fillOrigin = 0;
            Img_Mask.DOKill();
            Img_Mask.DOFillAmount(1, 0.2f);
        }

        protected override void OnBeforeDestroy()
        {
            
        }
    }
}
