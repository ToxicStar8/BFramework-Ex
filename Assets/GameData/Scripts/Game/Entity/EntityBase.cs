/*********************************************
 * 
 * 实体基类
 * 创建时间：2023/04/20 10:08:23
 *********************************************/
using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    public abstract class EntityBase : GameBase
    {
        #region 属性
        public GameObject gameObject;

        public Transform  transform;

        //剩余血量
        public int CurHp;
        //移速
        [SerializeField]
        public float Speed;
        #endregion

        #region 组件
        //动画
        public Animator Anim;
        //触发器
        public Collider2D CR;
        //刚体
        public Rigidbody2D Rg;
        #endregion

        #region 动画属性
        //动画X轴
        public string HorizontalStr = "Horizontal";
        //动画Y轴
        public string VerticalStr = "Vertical";
        //动画行走速度
        public string SpeedStr = "Speed";
        #endregion

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="loadHelper">加载器</param>
        /// <param name="prefabName">预制体名</param>
        /// <returns></returns>
        public static T CreateEntity<T>(LoadHelper loadHelper,string prefabName) where T : EntityBase,new()
        {
            //创建类对象池即可，游戏对象不会重复创建
            T entity = GameGod.Instance.PoolManager.CreateClassObj<T>();
            if(entity.gameObject == null)
            {
                var obj = loadHelper.LoadSync<GameObject>(prefabName);
                entity.gameObject = UnityEngine.Object.Instantiate(obj);
                entity.transform = entity.gameObject.GetComponent<Transform>();
                entity.OnInit();
            }
            entity.OnShow();
            return entity;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void OnInit()
        {
            CR = gameObject.GetComponent<Collider2D>();
            Anim = gameObject.GetComponent<Animator>();
            Rg = gameObject.GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// 显示
        /// </summary>
        public virtual void OnShow() { }
        /// <summary>
        /// Update
        /// </summary>
        public virtual void OnUpdate() { }
        /// <summary>
        /// 回收
        /// </summary>
        public virtual void OnDispose() { }
    }
}