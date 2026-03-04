/*********************************************
 * BFramework
 * 所有游戏对象的基类 存放通用方法
 * 创建时间：2023/05/08 17:01:23
 *********************************************/
using Cysharp.Threading.Tasks;
using MainPackage;
using System;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 所有游戏对象的Mono基类 同步GameBase里的方法
    /// </summary>
    public abstract class GameBaseMono : MonoBehaviour
    {
        #region Event 
        protected virtual void SendEvent(uint eventNo, params object[] args)
        {
            GameManager.Instance.EventManager.SendEvent(eventNo, args);
        }
        protected virtual void AddEventListener(uint eventNo, Action<object[]> callBack)
        {
            GameManager.Instance.EventManager.AddEventListener(eventNo, callBack);
        }
        protected virtual void RemoveEventListener(uint eventNo, Action<object[]> callBack)
        {
            GameManager.Instance.EventManager.RemoveEventListener(eventNo, callBack);
        }
        #endregion

        #region Pool
        /// <summary>
        /// 创建类对象池
        /// </summary>
        protected virtual ClassObjectPool<T> CreateClassPool<T>() where T : class, new()
        {
            return GameManager.Instance.PoolManager.CreateClassObjectPool<T>();
        }
        /// <summary>
        /// 在对象池中创建类对象
        /// </summary>
        protected virtual T CreateClassObj<T>() where T : class, new()
        {
            return GameManager.Instance.PoolManager.CreateClassObj<T>();
        }
        /// <summary>
        /// 回收类到池中
        /// </summary>
        protected virtual void RecycleClassObj<T>(T obj) where T : class, new()
        {
            GameManager.Instance.PoolManager.RecycleClassObj<T>(obj);
        }

        /// <summary>
        /// 创建游戏对象池
        /// </summary>
        protected virtual GameObjectPool CreateGameObjectPool(string objName)
        {
            return GameManager.Instance.PoolManager.CreateGameObjectPool(objName);
        }
        /// <summary>
        /// 在对象池中创建游戏对象
        /// </summary>
        protected virtual GameObject CreateGameObject(string objName, Transform trans = null)
        {
            return GameManager.Instance.PoolManager.CreateGameObject(objName, trans);
        }
        /// <summary>
        /// 回收游戏对象到池中
        /// </summary>
        protected virtual void RecycleGameObject(GameObject go)
        {
            GameManager.Instance.PoolManager.RecycleGameObject(go);
        }
        #endregion

        #region Table
        /// <summary>
        /// 获取配表
        /// </summary>
        protected virtual T GetTableCtrl<T>() where T : class, ITableCtrlBase
        {
            return GameManager.Instance.TableManager.GetTableCtrl<T>();
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
            GameManager.Instance.UIManager.OpenUI<T>(uiLevel, args);
        }

        /// <summary>
        /// 获取UI
        /// </summary>
        protected virtual UIBase GetUI<T>() where T : UIBase, new()
        {
            return GameManager.Instance.UIManager.GetUI<T>();
        }

        /// <summary>
        /// 隐藏UI
        /// </summary>
        protected virtual void HideUI<T>() where T : UIBase, new()
        {
            GameManager.Instance.UIManager.HideUI<T>();
        }

        /// <summary>
        /// 隐藏UI
        /// </summary>
        protected virtual void HideUI(string uiName)
        {
            GameManager.Instance.UIManager.HideUI(uiName);
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        protected virtual void CloseUI<T>() where T : UIBase, new()
        {
            GameManager.Instance.UIManager.CloseUI<T>();
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        protected virtual void CloseUI(string uiName)
        {
            GameManager.Instance.UIManager.CloseUI(uiName);
        }

        /// <summary>
        /// 关闭全部UI
        /// </summary>
        protected virtual void CloseAll()
        {
            GameManager.Instance.UIManager.CloseAll();
        }
        #endregion

        #region Audio
        protected virtual void PlayBGM(string audioName)
        {
            GameManager.Instance.AudioManager.PlayBackground(audioName);
        }

        protected virtual void PlaySound(string audioName)
        {
            GameManager.Instance.AudioManager.PlaySound(audioName);
        }
        #endregion

        #region Timer
        /// <summary>
        /// 添加定时器监听
        /// </summary>
        protected virtual void AddTimer(string timeName, TimerInfo timerInfo)
        {
            GameManager.Instance.TimerManager.AddTimer(timeName, timerInfo);
        }
        /// <summary>
        /// 添加一次性定时器监听，执行次数永远不能为-1，即无限，否则无限循环无法跳出
        /// </summary>
        protected virtual void AddTempTimer(TimerInfo timerInfo)
        {
            GameManager.Instance.TimerManager.AddTempTimer(timerInfo);
        }
        /// <summary>
        /// 获取定时器信息
        /// </summary>
        protected virtual TimerInfo GetTimerInfo(string timeName)
        {
            return GameManager.Instance.TimerManager.GetTimerInfo(timeName);
        }
        /// <summary>
        /// 移除定时器监听
        /// </summary>
        protected virtual void RemoveTimer(string timeName)
        {
            GameManager.Instance.TimerManager.RemoveTimer(timeName);
        }
        /// <summary>
        /// 移除定时器监听
        /// </summary>
        protected virtual void RemoveTimer(TimerInfo timerInfo)
        {
            GameManager.Instance.TimerManager.RemoveTimer(timerInfo.TimerName);
        }
        #endregion

        #region Fsm
        protected Fsm<T> CreateFsm<T>(T owner, FsmState<T>[] states) where T : class
        {
            var fsm = GameManager.Instance.FsmManager.CreateFsm<T>(owner, states);
            return fsm;
        }
        protected void ReleaseFsm(int fsmId)
        {
            GameManager.Instance.FsmManager.ReleaseFsm(fsmId);
        }
        #endregion

        #region Net
        protected virtual void HttpClearHeader()
        {
            GameManager.Instance.HttpManager.ClearHeader();
        }
        protected virtual void HttpAddHeader(string key, string value)
        {
            GameManager.Instance.HttpManager.AddHeader(key, value);
        }
        protected virtual HttpRoutine HttpGet(string url, Action<string> jsonDataCallBack = null, Action<string> errorCallBack = null)
        {
            return GameManager.Instance.HttpManager.Get(url, jsonDataCallBack, errorCallBack);
        }
        protected virtual HttpRoutine HttpGetTexture(string url, Action<Texture2D> texture2DCallBack = null, Action<string> errorCallBack = null)
        {
            return GameManager.Instance.HttpManager.GetTexture(url, texture2DCallBack, errorCallBack);
        }
        protected virtual HttpRoutine GetAudioClip(string url, AudioType audioType, Action<AudioClip> audioClipCallBack = null, Action<string> errorCallBack = null)
        {
            return GameManager.Instance.HttpManager.GetAudioClip(url, audioType, audioClipCallBack, errorCallBack);
        }
        protected virtual HttpRoutine HttpPost(string url, string json = null, Action<string> jsonDataCallBack = null, Action<string> errorCallBack = null)
        {
            return GameManager.Instance.HttpManager.Post(url, json, jsonDataCallBack, errorCallBack);
        }

        protected virtual void SocketClearHeader()
        {
            GameManager.Instance.SocketManager.ClearHeader();
        }
        protected virtual void SocketAddHeader(string key, string value)
        {
            GameManager.Instance.SocketManager.AddHeader(key, value);
        }
        protected virtual void SocketConnect(string wsUrl, Action openCallBack = null, Action closeCallBack = null)
        {
            GameManager.Instance.SocketManager.Connect(wsUrl, openCallBack, closeCallBack);
        }
        protected virtual void SocketSendMsg(string msg, Action<JsonData> callback)
        {
            GameManager.Instance.SocketManager.SendMsg(msg, callback);
        }
        protected virtual void SocketSendMsg(byte[] msg, Action<JsonData> callback)
        {
            GameManager.Instance.SocketManager.SendMsg(msg, callback);
        }
        protected virtual void SocketClose()
        {
            GameManager.Instance.SocketManager.CloseSocket();
        }
        #endregion

        #region Module
        protected virtual T GetModule<T>() where T : ModuleBase
        {
            return GameManager.Instance.ModuleManager.GetModule<T>();
        }
        #endregion

        #region Log
        protected virtual void Log(E_Log logType, string title = null, string content = null, string color = null)
        {
            GameManager.Instance.Log(logType, title, content, color);
        }
        protected virtual void Log(string content = null)
        {
            GameManager.Instance.Log(E_Log.Log, GetType().Name, content);
        }
        protected virtual void Log(string title = null, string content = null)
        {
            GameManager.Instance.Log(E_Log.Log, title, content);
        }
        protected virtual void LogError(string content = null)
        {
            GameManager.Instance.Log(E_Log.Error, GetType().Name, content);
        }
        protected virtual void LogError(string title = null, string content = null)
        {
            GameManager.Instance.Log(E_Log.Error, title, content);
        }
        protected virtual void LogWarning(string content = null)
        {
            GameManager.Instance.Log(E_Log.Warning, GetType().Name, content);
        }
        protected virtual void LogWarning(string title = null, string content = null)
        {
            GameManager.Instance.Log(E_Log.Warning, title, content);
        }
        protected virtual void LogException(string content = null)
        {
            GameManager.Instance.Log(E_Log.Exception, GetType().Name, content);
        }
        protected virtual void LogException(string title = null, string content = null)
        {
            GameManager.Instance.Log(E_Log.Exception, title, content);
        }
        #endregion

        #region Task
        protected virtual void AddTask(Action<TaskInfo> task)
        {
            GameManager.Instance.TaskManager.AddTask(task);
        }
        #endregion
    }
}
