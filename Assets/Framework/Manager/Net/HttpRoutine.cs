/*********************************************
 * BFramework
 * Http访问器
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using MainPackage;

namespace Framework
{
    /// <summary>
    /// Http访问器
    /// </summary>
    public class HttpRoutine
    {
        /// <summary>
        /// 所属对象池
        /// </summary>
        public ClassObjectPool<HttpRoutine> ThisPool;

        /// <summary>
        /// Http管理器
        /// </summary>
        public HttpManager HttpMgr => GameGod.Instance.HttpManager;

        /// <summary>
        /// Http请求回调
        /// </summary>
        private Action<string> _callBack;

        /// <summary>
        /// Http请求回调数据
        /// </summary>
        private string _jsonData;

        /// <summary>
        /// 是否繁忙
        /// </summary>
        public bool IsBusy { private set; get; }

        /// <summary>
        /// 当前重试次数
        /// </summary>
        private int _currRetry = 0;

        private string _url;
        private string _json;

        /// <summary>
        /// 发送的数据
        /// </summary>
        private Dictionary<string, object> _sendDic;

        public HttpRoutine()
        {
            _sendDic = new Dictionary<string, object>();
        }

        /// <summary>
        /// 外部调用的Get
        /// </summary>
        public void Get(string url, Action<string> callBack = null)
        {
            if (IsBusy)
            {
                GameEntry.Instance.Log(E_Log.Error, "网络锁");
                return;
            }

            IsBusy = true;

            _url = url;
            _callBack = callBack;

            GetUrl();
        }

        /// <summary>
        /// 外部调用的Post
        /// </summary>
        public void Post(string url, string json = null, Action<string> callBack = null)
        {
            if (IsBusy)
            {
                GameEntry.Instance.Log(E_Log.Error, "网络锁");
                return;
            }

            IsBusy = true;

            _url = url;
            _json = json;
            _callBack = callBack;

            PostUrl();
        }

        private void GetUrl()
        {
            GameEntry.Instance.Log(E_Log.Proto, string.Format("Get===><color=#00ffff>{0}</color>\n\r重试===><color=#00ffff>{1}</color>", _url, _currRetry));
            UnityWebRequest www = UnityWebRequest.Get(_url);
            GameEntry.Instance.StartCoroutine(SendRequest(www));
        }

        private void PostUrl()
        {
            //加密
            if (!string.IsNullOrWhiteSpace(_json))
            {
                //if (GameEntry.ParamsSettings.PostIsEncrypt && _currRetry == 0)
                //{
                //    _sendDic["value"] = _json;
                //    //web加密
                //    _sendDic["deviceIdentifier"] = DeviceUtil.DeviceIdentifier;
                //    _sendDic["deviceModel"] = DeviceUtil.DeviceModel;
                //    long t = GameEntry.Data.SysDataManager.CurrServerTime;
                //    _sendDic["sign"] = EncryptUtil.Md5(string.Format("{0}:{1}", t, DeviceUtil.DeviceIdentifier));
                //    _sendDic["t"] = t;

                //    _json = _sendDic.ToJson();
                //}

                //if (!string.IsNullOrWhiteSpace(GameEntry.ParamsSettings.PostContentType))
                //    www.SetRequestHeader("Content-Type", GameEntry.ParamsSettings.PostContentType);
            }

            //这里如果使用UnityWebRequest.Post再new UploadHandlerRaw，会造成内存泄漏
            UnityWebRequest www = new UnityWebRequest(_url, "POST");
            www.downloadHandler = new DownloadHandlerBuffer();
            www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(_json));
            www.SetRequestHeader("Content-Type", "application/json");
            GameEntry.Instance.Log(E_Log.Proto, string.Format("Post===><color=#00ffff>{0}</color>\n\r重试===><color=#00ffff>{1}</color>", _url + _json, _currRetry));
            GameEntry.Instance.StartCoroutine(SendRequest(www));
        }

        private IEnumerator SendRequest(UnityWebRequest www)
        {
            foreach (var item in HttpMgr.HttpHeaderDic)
            {
                www.SetRequestHeader(item.Key, item.Value);
            }
            www.timeout = 5;
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                //报错了 进行重试
                if (_currRetry > 0) yield return HttpMgr.WaitSeconds;
                _currRetry++;

                //如果还有重试次数 重试
                if (_currRetry <= HttpMgr.Retry)
                {
                    switch (www.method)
                    {
                        case UnityWebRequest.kHttpVerbGET:
                            GetUrl();
                            break;
                        case UnityWebRequest.kHttpVerbPOST:
                            PostUrl();
                            break;
                    }
                    yield break;
                }

                //结束
                IsBusy = false;
                _jsonData = www.error;
                //打印错误
                if (!string.IsNullOrWhiteSpace(_jsonData))
                {
                    GameEntry.Instance.Log(E_Log.Error, _jsonData);
                }
            }
            else
            {
                IsBusy = false;
                _jsonData = www.downloadHandler.text;
                //打印数据
                if (!string.IsNullOrWhiteSpace(_jsonData))
                {
                    GameEntry.Instance.Log(E_Log.Proto, string.Format("<color=#FFF11A>{{\"code\":{0},\"data\":{1}}}</color>", www.responseCode, _jsonData));
                }
            }
            //执行回调
            _callBack?.Invoke(_jsonData);
            //清理状态
            _callBack = null;
            _currRetry = 0;
            _url = null;
            _sendDic.Clear();
            _jsonData = null;
            www.Dispose();
            //回池
            ThisPool.Recycle(this);
        }
    }
}