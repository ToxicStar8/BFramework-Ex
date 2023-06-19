/*********************************************
 * 
 * 游戏管理器
 * 创建时间：2023/03/29 17:09:23
 *********************************************/
using Cinemachine;
using Framework;
using MainPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameData
{
    /// <summary>
    /// 游戏管理器
    /// </summary>
    public partial class GameManager : GameBase
    {
        public GameObject gameObject { private set; get; }
        public Transform transform { private set; get; }
        //单例
        public static GameManager Instance { private set; get; }
        //加载器
        public LoadHelper LoadHelper { private set; get; }
        //玩家控制器
        public PlayerCtrl PlayerCtrl { private set; get; }
        //主摄像机
        public Camera MainCamera { private set; get; }
        //虚拟摄像机
        public CinemachineVirtualCamera VirtualCamera { private set; get; }
        //是否初始化完毕
        private bool _initComplete = false;
        //开始时间
        public float _startTime { private set; get; }

        /// <summary>
        /// 创建游戏管理器
        /// </summary>
        public static void CreateGameManager()
        {
            Instance = new GameManager();
            //初始化加载器
            Instance.LoadHelper = LoadHelper.Create();
            //初始化游戏对象和Trans
            Instance.transform = GameEntry.Instance.GameStart;
            Instance.gameObject = Instance.transform.gameObject;
            //初始化主摄像机
            Instance.MainCamera = Instance.transform.Find("MainCamera").GetComponent<Camera>();
            Instance.VirtualCamera = Instance.transform.Find("PlayerFollowVirtualCamera").GetComponent<CinemachineVirtualCamera>();
            //添加Update监听
            GameEntry.Instance.UpdateCallback += Instance.OnUpdate;
            //添加关闭监听
            GameEntry.Instance.DisposeCallback += Instance.OnDispose;
        }

        /// <summary>
        /// 初始化游戏
        /// </summary>
        public void InitGame()
        {
            var tbRole = PlayerModule.PlayData.RoleData.GetTbRole();
            //初始化玩家
            PlayerCtrl = PlayerCtrl.CreateEntity<PlayerCtrl>(LoadHelper, tbRole.Asset);
            //摄像机跟随
            VirtualCamera.Follow = PlayerCtrl.transform;
            //游戏开始时间
            _startTime = Time.time;
            //地图初始化
            InitMap();
            //抽卡定时器
            var timeCountdown = GameGod.Instance.PoolManager.CreateClassObj<TimerInfo>();
            timeCountdown.Init(-1, 1, false, OnDrawCard);
            AddTimer(ConstDefine.DrawCardTimeKey, timeCountdown);
            //刷怪初始化
            InitMonsterInfo();
            //初始化完毕
            _initComplete = true;
        }

        /// <summary>
        /// 定时抽卡方法
        /// </summary>
        private void OnDrawCard()
        {
            PlayerModule.FnDrawCard(() =>
            {
                var playData = PlayerModule.PlayData;
                //如果抽空牌库
                if (playData.CurSelectIdx >= playData.CardHeldList.Count)
                {
                    Log(E_Log.Log, "重置牌序");
                    PlayerModule.FnReSortCardList(null);
                }
                //UI同步显示
                SendEven((ushort)UIEvent.OnUIMainShowCard, playData.CurSelectIdx.ToString());
            });
        }

        private void OnUpdate()
        {
            if (_initComplete)
            {
                PlayerCtrl.OnUpdate();
            }
        }

        public void OnDispose()
        {
            RemoveTimer(ConstDefine.DrawCardTimeKey);
            //RemoveCountdown(ConstDefine.CreateMonsterTimeKey);
            //回收加载器
            LoadHelper.Recycle(LoadHelper);
        }
    }
}