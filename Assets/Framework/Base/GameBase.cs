/*********************************************
 * BFramework
 * 所有游戏对象的基类 存放通用方法
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using Cysharp.Threading.Tasks;
using MainPackage;
using System;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 所有游戏对象的基类 存放通用方法
    /// </summary>
    public abstract class GameBase
    {
        #region Event 
        protected virtual void SendEvent(ushort eventNo, params object[] args)
        {
            GameGod.Instance.EventManager.SendEvent(eventNo, args);
        }
        protected virtual void AddEventListener(ushort eventNo, Action<object[]> callBack)
        {
            GameGod.Instance.EventManager.AddEventListener(eventNo, callBack);
        }
        protected virtual void RemoveEventListener(ushort eventNo, Action<object[]> callBack = null)
        {
            GameGod.Instance.EventManager.RemoveEventListener(eventNo, callBack);
        }
        #endregion

        #region Pool
        /// <summary>
        /// 创建类对象池
        /// </summary>
        protected virtual ClassObjectPool<T> CreateClassPool<T>() where T : class, new()
        {
            return GameGod.Instance.PoolManager.CreateClassObjectPool<T>();
        }
        /// <summary>
        /// 在对象池中创建类对象
        /// </summary>
        protected virtual T CreateClassObj<T>() where T : class, new()
        {
            return GameGod.Instance.PoolManager.CreateClassObj<T>();
        }
        /// <summary>
        /// 回收类到池中
        /// </summary>
        protected virtual void RecycleClassObj<T>(T obj) where T : class, new()
        {
            GameGod.Instance.PoolManager.RecycleClassObj<T>(obj);
        }

        /// <summary>
        /// 创建游戏对象池
        /// </summary>
        protected virtual GameObjectPool CreateGameObjectPool(string objName)
        {
            return GameGod.Instance.PoolManager.CreateGameObjectPool(objName);
        }
        /// <summary>
        /// 在对象池中创建游戏对象
        /// </summary>
        protected virtual GameObject CreateGameObject(string objName, Transform trans = null)
        {
            return GameGod.Instance.PoolManager.CreateGameObject(objName, trans);
        }
        /// <summary>
        /// 回收游戏对象到池中
        /// </summary>
        protected virtual void RecycleGameObject(GameObject go)
        {
            GameGod.Instance.PoolManager.RecycleGameObject(go);
        }
        #endregion

        #region Table
        /// <summary>
        /// 获取配表
        /// </summary>
        protected virtual T GetTableCtrl<T>() where T : class, ITableCtrlBase
        {
            return GameGod.Instance.TableManager.GetTableCtrl<T>();
        }
        #endregion

        #region Sprite
        /// <summary>
        /// 加载Sprite
        /// </summary>
        protected virtual Sprite GetSpriteSync(LoadHelper loadHelper, string atlasName, string spriteName)
        {
            var sp = loadHelper.GetSpriteSync(atlasName, spriteName);
            return sp;
        }
        /// <summary>
        /// 加载Sprite
        /// </summary>
        protected virtual async UniTask<Sprite> GetSpriteAsync(LoadHelper loadHelper, string atlasName, string spriteName)
        {
            var sp = await loadHelper.GetSpriteAsync(atlasName, spriteName);
            return sp;
        }
        #endregion

        #region UI
        /// <summary>
        /// 打开UI
        /// </summary>
        protected virtual void OpenUI<T>(E_UILevel uiLevel = E_UILevel.Common, params object[] args) where T : UIBase, new()
        {
            GameGod.Instance.UIManager.OpenUI<T>(uiLevel, args);
        }

        /// <summary>
        /// 获取UI
        /// </summary>
        protected virtual UIBase GetUI<T>() where T : UIBase, new()
        {
            return GameGod.Instance.UIManager.GetUI<T>();
        }

        /// <summary>
        /// 隐藏UI
        /// </summary>
        protected virtual void HideUI<T>() where T : UIBase, new()
        {
            GameGod.Instance.UIManager.HideUI<T>();
        }

        /// <summary>
        /// 隐藏UI
        /// </summary>
        protected virtual void HideUI(string uiName)
        {
            GameGod.Instance.UIManager.HideUI(uiName);
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        protected virtual void CloseUI<T>() where T : UIBase, new()
        {
            GameGod.Instance.UIManager.CloseUI<T>();
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        protected virtual void CloseUI(string uiName)
        {
            GameGod.Instance.UIManager.CloseUI(uiName);
        }

        /// <summary>
        /// 关闭全部UI
        /// </summary>
        protected virtual void CloseAll()
        {
            GameGod.Instance.UIManager.CloseAll();
        }
        #endregion

        #region Audio
        protected virtual void PlayBGM(string audioName)
        {
            GameGod.Instance.AudioManager.PlayBackground(audioName);
        }

        protected virtual void PlaySound(string audioName)
        {
            GameGod.Instance.AudioManager.PlaySound(audioName);
        }
        #endregion

        #region Timer
        /// <summary>
        /// 添加定时器监听
        /// </summary>
        protected virtual void AddTimer(string timeName, TimerInfo timerInfo)
        {
            GameGod.Instance.TimeManager.AddTimer(timeName, timerInfo);
        }
        /// <summary>
        /// 添加一次性定时器监听，执行次数永远不能为-1，即无限，否则无限循环无法跳出
        /// </summary>
        protected virtual void AddTempTimer(TimerInfo timerInfo)
        {
            GameGod.Instance.TimeManager.AddTempTimer(timerInfo);
        }
        /// <summary>
        /// 获取定时器信息
        /// </summary>
        protected virtual TimerInfo GetTimerInfo(string timeName)
        {
            return GameGod.Instance.TimeManager.GetTimerInfo(timeName);
        }
        /// <summary>
        /// 移除定时器监听
        /// </summary>
        protected virtual void RemoveTimer(string timeName, TimerInfo timerInfo = null)
        {
            GameGod.Instance.TimeManager.RemoveTimer(timeName);
        }
        #endregion

        #region Fsm
        protected Fsm<T> CreateFsm<T>(T owner, FsmState<T>[] states) where T : class
        {
            var fsm = GameGod.Instance.FsmManager.CreateFsm<T>(owner, states);
            return fsm;
        }
        protected void RelaseFsm(int fsmId)
        {
            GameGod.Instance.FsmManager.RelaseFsm(fsmId);
        }
        #endregion

        #region Net
        protected virtual void HttpClearHeader()
        {
            GameGod.Instance.HttpManager.ClearHeader();
        }
        protected virtual void HttpAddHeader(string key, string value)
        {
            GameGod.Instance.HttpManager.AddHeader(key, value);
        }
        protected virtual HttpRoutine HttpGet(string url, Action<string> callBack = null)
        {
            return GameGod.Instance.HttpManager.Get(url, callBack);
        }
        protected virtual HttpRoutine HttpGetTexture(string url, Action<Texture2D> callBack = null)
        {
            return GameGod.Instance.HttpManager.GetTexture(url, callBack);
        }
        protected virtual HttpRoutine GetAudioClip(string url, AudioType audioType, Action<AudioClip> callBack = null)
        {
            return GameGod.Instance.HttpManager.GetAudioClip(url, audioType, callBack);
        }
        protected virtual HttpRoutine HttpPost(string url, string json = null, Action<string> callBack = null)
        {
            return GameGod.Instance.HttpManager.Post(url, json, callBack);
        }

        protected virtual void SocketClearHeader()
        {
            GameGod.Instance.SocketManager.ClearHeader();
        }
        protected virtual void SocketAddHeader(string key, string value)
        {
            GameGod.Instance.SocketManager.AddHeader(key, value);
        }
        protected virtual void SocketConnect(string wsUrl, Action openCallBack = null, Action closeCallBack = null)
        {
            GameGod.Instance.SocketManager.Connect(wsUrl, openCallBack, closeCallBack);
        }
        protected virtual void SocketSendMsg(string msg, Action<JsonData> callback)
        {
            GameGod.Instance.SocketManager.SendMsg(msg, callback);
        }
        protected virtual void SocketSendMsg(byte[] msg, Action<JsonData> callback)
        {
            GameGod.Instance.SocketManager.SendMsg(msg, callback);
        }
        protected virtual void SocketClose()
        {
            GameGod.Instance.SocketManager.CloseSocket();
        }
        #endregion

        #region Module
        protected virtual T GetModule<T>() where T : ModuleBase
        {
            return GameGod.Instance.ModuleManager.GetModule<T>();
        }
        #endregion

        #region Log
        protected virtual void Log(E_Log logType, string title = null, string content = null)
        {
            GameGod.Instance.Log(logType, title, content);
        }
        protected virtual void Log(string content = null)
        {
            GameGod.Instance.Log(E_Log.Log, GetType().Name, content);
        }
        protected virtual void Log(string title = null, string content = null)
        {
            GameGod.Instance.Log(E_Log.Log, title, content);
        }
        protected virtual void LogError(string content = null)
        {
            GameGod.Instance.Log(E_Log.Error, GetType().Name, content);
        }
        protected virtual void LogError(string title = null, string content = null)
        {
            GameGod.Instance.Log(E_Log.Error, title, content);
        }
        protected virtual void LogWarring(string content = null)
        {
            GameGod.Instance.Log(E_Log.Warring, GetType().Name, content);
        }
        protected virtual void LogWarring(string title = null, string content = null)
        {
            GameGod.Instance.Log(E_Log.Warring, title, content);
        }
        #endregion

        #region Task
        protected virtual void AddTask(Action<TaskInfo> task)
        {
            GameGod.Instance.TaskManager.AddTask(task);
        }
        #endregion
    }
}