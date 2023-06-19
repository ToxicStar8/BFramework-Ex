/*********************************************
 * 
 * 玩家控制器
 * 创建时间：2023/03/29 17:09:23
 *********************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    public partial class PlayerCtrl : EntityBase
    {
        private TableRole _tbRole => PlayerModule.PlayData.GetTbRole();

        public override void OnInit()
        {
            base.OnInit();
        }

        public override void OnShow()
        {
            base.OnShow();
            transform.position = Vector3.zero;
            Speed = _tbRole.Speed;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            PlayMove();
        }

        /// <summary>
        /// 玩家位移
        /// </summary>
        private void PlayMove()
        {
            //获取输入
            var x = Input.GetAxisRaw(HorizontalStr);
            var y = Input.GetAxisRaw(VerticalStr);
            //手柄模式的输入
            //var x = JoyHandle.Instance.Horizontal;
            //var y = JoyHandle.Instance.Vertical;

            //动画混合树使用 用于四向行走动画
            //bool isStatic = x == 0 && y == 0;
            //if (!isStatic)
            //{
            //    _anim.SetFloat(HORIZONTAL, x);
            //    _anim.SetFloat(VERTICAL, y);
            //}

            //更新速度
            Anim.SetFloat(SpeedStr, Mathf.Abs(x) + Mathf.Abs(y));

            //更新面向
            if (x != 0)
            {
                transform.localScale = new Vector3(x, 1, 1);
            }

            //用刚体进行位移 不然发生碰撞屏幕会颤抖
            Vector3 motion = transform.right * x + transform.up * y;
            //x和y=1 斜向为根号2，所以需要乘以(根号1/2)=(根号2)/2，根号2*(根号2)/2=1
            if (x != 0 && y != 0)
            {
                motion *= Mathf.Sqrt(0.5f);
            }
            var velocity = motion * Speed;
            Rg.velocity = velocity;
        }
    }
}