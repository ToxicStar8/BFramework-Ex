/*********************************************
 * BFramework
 * Http管理器
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using MainPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Http管理器
    /// </summary>
    public class HttpManager : ManagerBase
    {
        /// <summary>
        /// 连接失败后重试次数
        /// </summary>
        public int Retry { get; private set; }

        /// <summary>
        /// 连接失败后重试间隔（秒）
        /// </summary>
        public int RetryInterval { get; private set; }

        /// <summary>
        /// 协程里重试间隔
        /// </summary>
        public WaitForSeconds WaitSeconds { get; private set; }

        /// <summary>
        /// Token 用于适配不同的后端要求 例：Token、Authorization等
        /// </summary>
        public Dictionary<string, string> HttpHeaderDic { get; private set; }

        /// <summary>
        /// 网络请求获取的图片
        /// </summary>
        public Dictionary<string, Texture2D> HttpTextureDic { get; private set; }

        /// <summary>
        /// 网络请求获取的音频
        /// </summary>
        public Dictionary<string, AudioClip> HttpAudioClipDic { get; private set; }

        public override void OnInit()
        {
            Retry = 0;
            RetryInterval = 0;
            WaitSeconds = new WaitForSeconds(RetryInterval);
            HttpHeaderDic = new Dictionary<string, string>();
            HttpTextureDic = new Dictionary<string, Texture2D>();
            HttpAudioClipDic = new Dictionary<string, AudioClip>();
        }

        /// <summary>
        /// 清空浏览器请求标头
        /// </summary>
        public void ClearHeader()
        {
            HttpHeaderDic.Clear();
            GameGod.Instance.Log(E_Log.Framework, "清空浏览器标头");
        }

        /// <summary>
        /// 添加浏览器请求标头
        /// </summary>
        public void AddHeader(string key, string value)
        {
            HttpHeaderDic.Add(key, value);
            GameGod.Instance.Log(E_Log.Framework, "添加浏览器标头" + key, value);
        }

        /// <summary>
        /// Get数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="jsonDataCallBack"></param>
        /// <returns></returns>
        public HttpRoutine Get(string url, Action<string> jsonDataCallBack = null, Action<string> errorCallBack = null)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                GameGod.Instance.Log(E_Log.Error, "请求的url为空，url" + url);
                return null;
            }

            var pool = GameGod.Instance.PoolManager.CreateClassObjectPool<HttpRoutine>();
            var routine = pool.CreateClassObj();
            routine.ThisPool = pool;
            routine.Get(url, jsonDataCallBack, errorCallBack);
            return routine;
        }

        /// <summary>
        /// Get贴图
        /// </summary>
        /// <param name="url"></param>
        /// <param name="texture2DCallBack"></param>
        /// <returns></returns>
        public HttpRoutine GetTexture(string url, Action<Texture2D> texture2DCallBack = null, Action<string> errorCallBack = null)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                GameGod.Instance.Log(E_Log.Error, "请求的url为空，url" + url);
                return null;
            }

            if (HttpTextureDic.TryGetValue(url, out var texture2D))
            {
                GameGod.Instance.Log(E_Log.Framework, "请求的url图片已存在，直接返回图片");
                texture2DCallBack?.Invoke(texture2D);
                return null;
            }

            var pool = GameGod.Instance.PoolManager.CreateClassObjectPool<HttpRoutine>();
            var routine = pool.CreateClassObj();
            routine.ThisPool = pool;
            routine.Get(url, (texture2D) =>
            {
                if (texture2D == null)
                {
                    GameGod.Instance.Log(E_Log.Error, "请求的url返回图片为空，url" + url);
                    return;
                }
                HttpTextureDic[url] = texture2D;
                texture2DCallBack?.Invoke(texture2D);
            }, errorCallBack);
            return routine;
        }

        /// <summary>
        /// Get音频
        /// </summary>
        /// <param name="url"></param>
        /// <param name="audioType"></param>
        /// <param name="audioClipCallBack"></param>
        /// <returns></returns>
        public HttpRoutine GetAudioClip(string url, AudioType audioType, Action<AudioClip> audioClipCallBack = null, Action<string> errorCallBack = null)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                GameGod.Instance.Log(E_Log.Error, "请求的url为空，url" + url);
                return null;
            }

            if (HttpAudioClipDic.TryGetValue(url, out var audioClip))
            {
                GameGod.Instance.Log(E_Log.Framework, "请求的url音频已存在，直接返回音频");
                audioClipCallBack?.Invoke(audioClip);
                return null;
            }

            var pool = GameGod.Instance.PoolManager.CreateClassObjectPool<HttpRoutine>();
            var routine = pool.CreateClassObj();
            routine.ThisPool = pool;
            routine.Get(url, audioType, (audioClip) =>
            {
                if (audioClip == null)
                {
                    GameGod.Instance.Log(E_Log.Error, "请求的url返回音频为空，url" + url);
                    return;
                }
                HttpAudioClipDic[url] = audioClip;
                audioClipCallBack?.Invoke(audioClip);
            }, errorCallBack);
            return routine;
        }

        public HttpRoutine Post(string url, string json = null, Action<string> jsonDataCallBack = null, Action<string> errorCallBack = null)
        {
            var pool = GameGod.Instance.PoolManager.CreateClassObjectPool<HttpRoutine>();
            var routine = pool.CreateClassObj();
            routine.ThisPool = pool;
            routine.Post(url, json, jsonDataCallBack,errorCallBack);
            return routine;
        }

        public override void OnUpdate() { }
        public override void OnDispose() { }
    }
}
