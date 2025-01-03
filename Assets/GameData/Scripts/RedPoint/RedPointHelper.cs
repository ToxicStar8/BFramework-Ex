/*********************************************
 * 
 * 红点业务层帮助类
 * 创建时间：2025/01/03 21:55:23
 *********************************************/
using Cysharp.Threading.Tasks;
using Framework;
using GameData;
using MainPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GameData
{
    /// <summary>
    /// 红点业务层帮助类
    /// todo:后面改为不用单例
    /// </summary>
    public class RedPointHelper : InstanceBase<RedPointHelper>
    {
        public void OnInit()
        {
            //启动时将红点刷新事件注册到游戏中，不再调整
            RegisterUpdateCallback();
        }

        /// <summary>
        /// 监听所有更改事件
        /// </summary>
        public void RegisterUpdateCallback()
        {
            AddEventListener((ushort)RedPointEvent.RedPointTest, Update_001);
        }


        #region Update
        //主菜单宠物头像红点 获得新宠物时更新
        private void Update_001(params object[] args)
        {
            int id = 1;
            //算完红点数量记得到Manager设置一下
            var num = GetAnyNewPet();
            CommoneSetNumAndInvoke(id, "", num);
        }
        //获取任意新宠物数量，无新宠物为0 有新宠物为1
        private int GetAnyNewPet()
        {
            var num = 0;
            //var petDatas = GetData<MyPet>();
            //for (int i = 0; i < petDatas.Pets.Count; i++)
            //{
            //    var pet = petDatas.Pets[i];
            //    if (pet.IsNewGetPet)
            //    {
            //        num = 1;
            //        break;
            //    }
            //}
            return num;
        }
        #endregion

        /// <summary>
        /// Update事件回调中，通用的设置数量并且执行对应Id的回调
        /// </summary>
        private void CommoneSetNumAndInvoke(int id, string key, int num)
        {
            GameGod.Instance.RedPointManager.SetRedNumById(id, num);
            var callBack = GameGod.Instance.RedPointManager.GetCallBack(id);
            callBack?.Invoke(id, key, num);
        }

        /// <summary>
        /// 等待红点初始化完毕
        /// </summary>
        public async UniTask WaitRedPointInitComplete(RedPointBase redPointBase)
        {
            while (!redPointBase.IsInit)
            {
                await UniTask.DelayFrame(1);
            }
        }

        /// <summary>
        /// 获取指定类型的红点数量 -1 表示还未初始化 0 表示无 N 表示几个
        /// </summary>
        public int GetNumById(int id, string key)
        {
            var num = GameGod.Instance.RedPointManager.GetRedNumById(id);
            if (num == -1)
            {
                num = InitRedPoint(id, key);
            }
            return num;
        }

        /// <summary>
        /// 初始化时执行业务逻辑计算并且赋值
        /// </summary>
        private int InitRedPoint(int id, string key)
        {
            int num = -1;
            switch (id)
            {
                case 1:
                    {
                        num = GetAnyNewPet();
                    }
                    break;

                default:
                    Log(E_Log.Error, "未知红点类型，Id=" + id.ToString());
                    return -1;
            }

            GameGod.Instance.RedPointManager.SetRedNumById(id, num);
            return num;
        }
    }
}