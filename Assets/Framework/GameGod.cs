/*********************************************
 * BFramework
 * 游戏入口
 * 创建时间：2022/12/25 20:40:23
 *********************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 游戏总控制器
    /// </summary>
    public class GameGod : MonoBehaviour
    {
        public static GameGod Instance { private set; get; }
        public PoolManager PoolManager { private set; get; }
        public HttpManager HttpManager { private set; get; }
        public SocketManager SocketManager { private set; get; }
        public ABManager ABManager { private set; get; }
        public UIManager UIManager { private set; get; }
        public LoadManager LoadManager { private set; get; }
        public EventManager EventManager { private set; get; }
        public TableManager TableManager { private set; get; }
        public AudioManager AudioManager { private set; get; }
        public TimerManager TimeManager { private set; get; }
        //public ModuleManager ModuleManager { private set; get; }       
        public FsmManager FsmManager { private set; get; }

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }

        private void Start()
        {
            //限定60fps
            Application.targetFrameRate = 60;

            PoolManager = new PoolManager();
            HttpManager = new HttpManager();
            SocketManager = new SocketManager();
            ABManager = new ABManager();
            UIManager = new UIManager();
            LoadManager = new LoadManager();
            EventManager = new EventManager();
            TableManager = new TableManager();
            AudioManager = new AudioManager();
            TimeManager = new TimerManager();
            FsmManager = new FsmManager();
        }

        private void Update()
        {
            PoolManager.OnUpdate();
            HttpManager.OnUpdate();
            SocketManager.OnUpdate();
            ABManager.OnUpdate();
            UIManager.OnUpdate();
            LoadManager.OnUpdate();
            EventManager.OnUpdate();
            TableManager.OnUpdate();
            AudioManager.OnUpdate();
            TimeManager.OnUpdate();
            //ModuleManager.OnUpdate();
            FsmManager.OnUpdate();
        }

        private void OnApplicationQuit()
        {
            //先执行

        }

        private void OnDestroy()
        {
            //再执行
            ABManager.OnDispose();
            FsmManager.OnDispose();
            UIManager.OnDispose();
            PoolManager.OnDispose();
            LoadManager.OnDispose();
            EventManager.OnDispose();
            TableManager.OnDispose();
            HttpManager.OnDispose();
            SocketManager.OnDispose();
            AudioManager.OnDispose();
            TimeManager.OnDispose();
        }
    }
}