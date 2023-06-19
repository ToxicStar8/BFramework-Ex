/*********************************************
 * BFramework
 * ״̬��״̬��
 * ����ʱ�䣺2023/04/06 15:00:23
 *********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Framework
{
    /// <summary>
    /// ״̬����״̬
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class FsmState<T> where T : class
    {
        /// <summary>
        /// ���и�״̬��״̬��
        /// </summary>
        public Fsm<T> CurrFsm;

        public abstract void OnInit();
        /// <summary>
        /// ����״̬
        /// </summary>
        public abstract void OnEnter();

        /// <summary>
        /// ִ��״̬
        /// </summary>
        public abstract void OnUpdate();

        /// <summary>
        /// �뿪״̬
        /// </summary>
        public abstract void OnLeave();

        /// <summary>
        /// ״̬������ʱ����
        /// </summary>
        public abstract void OnDestroy();

    }
}