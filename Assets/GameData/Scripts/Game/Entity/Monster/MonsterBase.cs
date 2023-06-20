/*********************************************
 * 
 * 怪物基类
 * 创建时间：2023/04/20 10:27:23
 *********************************************/
using Framework;
using MainPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    public abstract class MonsterBase : EntityBase
    {
        public PlayerCtrl PlayerCtrl => GameManager.Instance.PlayerCtrl;

        public Fsm<MonsterBase> Fsm;

        public override void OnInit()
        {
            base.OnInit();
            Fsm = CreateFsm(this,new FsmState<MonsterBase>[] { new FsmMonsterMove()});
        }

        public override void OnShow()
        {
            base.OnShow();
            Fsm.ChangeState(0);
        }

        public override void OnDispose()
        {
            base.OnDispose();
            Fsm.ChangeState(-1);
            gameObject.SetParent(GameEntry.Instance.ObjPool);
        }
    }

    public class FsmMonsterMove : FsmState<MonsterBase>
    {
        public override void OnInit()
        {
            CurrFsm.Owner.Log(E_Log.Log, "初始化FsmMonsterMove");
        }
        public override void OnEnter()
        {
            CurrFsm.Owner.Log(E_Log.Log, "进入FsmMonsterMove");
        }
        public override void OnUpdate()
        {
            var owner = CurrFsm.Owner;
            //方向
            var vector = (owner.PlayerCtrl.transform.position - owner.transform.position).normalized;
            //用刚体进行位移
            owner.Rg.velocity = vector * owner.Speed;
            //更新动画里的速度值
            owner.Anim.SetFloat(owner.SpeedStr, vector.magnitude);
            //更新面向
            if(vector.x != 0)
            {
                owner.transform.SetLocalScaleX(vector.x < 0 ? -1 : 1);
            }
            //Debug.Log(owner.Rg.velocity.magnitude);
        }
        public override void OnLeave()
        {
            CurrFsm.Owner.Log(E_Log.Log, "离开FsmMonsterMove");
        }
        public override void OnDestroy()
        {
            CurrFsm.Owner.Log(E_Log.Log, "销毁FsmMonsterMove");
        }
    }
}