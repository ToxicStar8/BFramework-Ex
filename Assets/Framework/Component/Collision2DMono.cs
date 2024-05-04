/*********************************************
 * BFramework
 * 碰撞绑定类
 * 创建时间：2023/05/10 13:49:23
 *********************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class Collision2DMono : MonoBehaviour
    {
        public Action<Collision2D> OnTrigger2DEnterCallback;
        public Action<Collision2D> OnTrigger2DStayCallback;
        public Action<Collision2D> OnTrigger2DExitCallback;

        //碰撞器器原生方法
        private void OnCollisionEnter2D(Collision2D collision)
        {
            OnTrigger2DEnterCallback?.Invoke(collision);
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            OnTrigger2DStayCallback?.Invoke(collision);
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            OnTrigger2DExitCallback?.Invoke(collision);
        }
    }
}
