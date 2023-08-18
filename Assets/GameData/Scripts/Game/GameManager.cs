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
    public partial class GameManager : InstanceBase<GameManager>
    {
        public GameObject gameObject { private set; get; }
        public Transform transform { private set; get; }
        //加载器
        public LoadHelper LoadHelper { private set; get; }
        //玩家控制器
        //public PlayerCtrl PlayerCtrl { private set; get; }
        //主摄像机
        public Camera MainCamera { private set; get; }
        //虚拟摄像机
        public CinemachineVirtualCamera VirtualCamera { private set; get; }
        //是否初始化完毕
        //private bool _initComplete = false;
        //开始时间
        public float _startTime { private set; get; }

        /// <summary>
        /// 创建游戏管理器
        /// </summary>
        public static void CreateGameManager()
        {
            //初始化加载器
            Instance.LoadHelper = LoadHelper.Create();
            //切换游戏场景
            Instance.LoadHelper.LoadSceneSync("Scene_Game");
            //添加Update监听
            GameGod.Instance.UpdateCallback += Instance.OnUpdate;
            //添加关闭监听
            GameGod.Instance.DisposeCallback += Instance.OnDispose;
        }

        /// <summary>
        /// 初始化游戏
        /// </summary>
        public void InitGame()
        {
            //初始化游戏对象和Trans
            gameObject = GameObject.Find("GameStart");
            transform = gameObject.transform;
            //初始化主摄像机
            MainCamera = transform.Find("MainCamera").GetComponent<Camera>();
            VirtualCamera = transform.Find("PlayerFollowVirtualCamera").GetComponent<CinemachineVirtualCamera>();
            //初始化玩家
            //PlayerCtrl = PlayerCtrl.CreateEntity<PlayerCtrl>(LoadHelper, "Player.prefab");
            //摄像机跟随
            //VirtualCamera.Follow = PlayerCtrl.transform;
            //游戏开始时间
            _startTime = Time.time;
            //初始化完毕
            //_initComplete = true;
        }

        private void OnUpdate()
        {
            //PlayerCtrl?.OnUpdate();
        }

        public void OnDispose()
        {
            //回收加载器
            LoadHelper.Recycle(LoadHelper);
        }
    }
}