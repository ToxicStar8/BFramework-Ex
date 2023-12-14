/*********************************************
 * BFramework
 * 游戏框架总控制器
 * 创建时间：2022/12/25 20:40:23
 *********************************************/
using MainPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 游戏框架总控制器
    /// </summary>
    public class GameGod : MonoBehaviour
    {
        /// <summary>
        /// Update回调
        /// </summary>
        public Action UpdateCallback;

        /// <summary>
        /// 退出回调回调
        /// </summary>
        public Action DisposeCallback;

        /// <summary>
        /// 全局用的加载器，基本不释放
        /// </summary>
        public LoadHelper LoadHelper { private set; get; }

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
        public DataManager DataManager { private set; get; }
        public FsmManager FsmManager { private set; get; }
        public ModuleManager ModuleManager { private set; get; }
        public RedPointManager RedPointManager { private set; get; }


        private void Awake()
        {
            //限定60fps
            Application.targetFrameRate = 60;

            Instance = this;
            DontDestroyOnLoad(Instance);

            ABManager = new ABManager();
            LoadManager = new LoadManager();
            PoolManager = new PoolManager();
            HttpManager = new HttpManager();
            SocketManager = new SocketManager();
            UIManager = new UIManager();
            ModuleManager = new ModuleManager();
            EventManager = new EventManager();
            TableManager = new TableManager();
            AudioManager = new AudioManager();
            TimeManager = new TimerManager();
            DataManager = new DataManager();
            FsmManager = new FsmManager();
            RedPointManager = new RedPointManager();

            LoadHelper = LoadHelper.Create();
        }

        private void Start()
        {

        }

        private void Update()
        {
            PoolManager.OnUpdate();
            HttpManager.OnUpdate();
            SocketManager.OnUpdate();
            ABManager.OnUpdate();
            UIManager.OnUpdate();
            LoadManager.OnUpdate();
            ModuleManager.OnUpdate();
            EventManager.OnUpdate();
            TableManager.OnUpdate();
            AudioManager.OnUpdate();
            TimeManager.OnUpdate();
            DataManager.OnUpdate();
            FsmManager.OnUpdate();
            RedPointManager.OnUpdate();
            UpdateCallback?.Invoke();
        }

        private void OnApplicationQuit()
        {
            //先执行
            DisposeCallback?.Invoke();
        }

        private void OnDestroy()
        {
            //再执行
            DataManager.OnDispose();
            ABManager.OnDispose();
            FsmManager.OnDispose();
            UIManager.OnDispose();
            PoolManager.OnDispose();
            LoadManager.OnDispose();
            ModuleManager.OnDispose();
            EventManager.OnDispose();
            TableManager.OnDispose();
            HttpManager.OnDispose();
            SocketManager.OnDispose();
            AudioManager.OnDispose();
            RedPointManager.OnDispose();
            TimeManager.OnDispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public Transform GameStart => GameEntry.Instance.GameStart;

        /// <summary>
        /// 获得UI根节点下的层级节点
        /// </summary>
        /// <param name="uiLevel"></param>
        /// <returns></returns>
        public RectTransform GetUILevelTrans(E_UILevel uiLevel) => GameEntry.Instance.GetUILevelTrans(uiLevel);

        /// <summary>
        /// Log
        /// </summary>
        public void Log(E_Log logType, string title = null, string content = null,string color = null) => GameEntry.Instance.Log(logType, title, content,color);
    }
}