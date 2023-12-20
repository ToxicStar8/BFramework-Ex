/*********************************************
 * BFramework
 * Http访问器
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using MainPackage;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

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
        /// Http文本请求回调
        /// </summary>
        private Action<string> _jsonDataCallBack;

        /// <summary>
        /// Http图片请求回调
        /// </summary>
        private Action<Texture2D> _Texture2DCallBack;

        /// <summary>
        /// Http音频请求回调
        /// </summary>
        private Action<AudioClip> _AudioClipCallBack;

        /// <summary>
        /// 是否繁忙
        /// </summary>
        public bool IsBusy { private set; get; }

        /// <summary>
        /// 当前重试次数
        /// </summary>
        private int _currRetry = 0;

        /// <summary>
        /// URL
        /// </summary>
        private string _url;

        /// <summary>
        /// Post发送的数据
        /// </summary>
        private string _json;

        /// <summary>
        /// 
        /// </summary>
        private UnityWebRequest _webRequest;
        public UnityWebRequest GetWWW() => _webRequest;

        public HttpRoutine()
        {

        }

        /// <summary>
        /// 外部调用的Get 0=普通Get 1=TextureGet
        /// </summary>
        public void Get(string url, Action<string> callBack = null)
        {
            if (IsBusy)
            {
                GameGod.Instance.Log(E_Log.Error, "网络锁");
                return;
            }

            IsBusy = true;

            _url = url;
            _jsonDataCallBack = callBack;
            _Texture2DCallBack = null;
            _AudioClipCallBack = null;
            _webRequest?.Dispose();

            GetUrl();
        }

        public void Get(string url, Action<Texture2D> callBack = null)
        {
            if (IsBusy)
            {
                GameGod.Instance.Log(E_Log.Error, "网络锁");
                return;
            }

            IsBusy = true;

            _url = url;
            _jsonDataCallBack = null;
            _Texture2DCallBack = callBack;
            _AudioClipCallBack = null;
            _webRequest?.Dispose();

            GetTexture();
        }

        public void Get(string url, AudioType audioType, Action<AudioClip> callBack = null)
        {
            if (IsBusy)
            {
                GameGod.Instance.Log(E_Log.Error, "网络锁");
                return;
            }

            IsBusy = true;

            _url = url;
            _jsonDataCallBack = null;
            _Texture2DCallBack = null;
            _AudioClipCallBack = callBack;
            _webRequest?.Dispose();

            GetAudioClip(audioType);
        }

        /// <summary>
        /// 外部调用的Post
        /// </summary>
        public void Post(string url, string json = null, Action<string> callBack = null)
        {
            if (IsBusy)
            {
                GameGod.Instance.Log(E_Log.Error, "网络锁");
                return;
            }

            IsBusy = true;

            _url = url;
            _json = json;
            _jsonDataCallBack = callBack;
            _webRequest?.Dispose();

            PostUrl();
        }

        private void GetUrl()
        {
            GameGod.Instance.Log(E_Log.Proto, string.Format("Get===>{0}\n\r重试===>{1}", _url, _currRetry));
            _webRequest = UnityWebRequest.Get(_url);
            GameGod.Instance.StartCoroutine(SendRequest());
        }

        private void GetTexture()
        {
            GameGod.Instance.Log(E_Log.Proto, string.Format("Get===>{0}\n\r重试===>{1}", _url, _currRetry));
            _webRequest = UnityWebRequestTexture.GetTexture(_url);
            GameGod.Instance.StartCoroutine(SendRequest());
        }

        private void GetAudioClip(AudioType audioType)
        {
            GameGod.Instance.Log(E_Log.Proto, string.Format("Get===>{0}\n\r重试===>{1}", _url, _currRetry));
            _webRequest = UnityWebRequestMultimedia.GetAudioClip(_url, audioType);
            GameGod.Instance.StartCoroutine(SendRequest());
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
            _webRequest = new UnityWebRequest(_url, "POST");
            _webRequest.downloadHandler = new DownloadHandlerBuffer();
            _webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(_json));
            _webRequest.SetRequestHeader("Content-Type", "application/json");
            GameGod.Instance.Log(E_Log.Proto, string.Format("Post===>{0}\n\r重试===>{1}", _url + _json, _currRetry));
            GameGod.Instance.StartCoroutine(SendRequest());
        }

        private IEnumerator SendRequest()
        {
            foreach (var item in HttpMgr.HttpHeaderDic)
            {
                _webRequest.SetRequestHeader(item.Key, item.Value);
            }

            _webRequest.timeout = 10;
            yield return _webRequest.SendWebRequest();
            if (_webRequest.result == UnityWebRequest.Result.ConnectionError || _webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                //报错了 进行重试
                if (_currRetry > 0) yield return HttpMgr.WaitSeconds;
                _currRetry++;

                //如果还有重试次数 重试
                if (_currRetry <= HttpMgr.Retry)
                {
                    switch (_webRequest.method)
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

                //打印错误
                if (!string.IsNullOrWhiteSpace(_webRequest.error))
                {
                    GameGod.Instance.Log(E_Log.Error, _webRequest.error);
                }
            }
            else
            {
                var downloadHandler = _webRequest.downloadHandler;
                //通过委托回调是否为空 来判断是请求什么玩楞
                if(_jsonDataCallBack != null)
                {
                    //数据回调
                    GameGod.Instance.Log(E_Log.Proto, string.Format("<color=#FFF11A>{{\"code\":{0},\"data\":{1}}}</color>", _webRequest.responseCode, downloadHandler.text));
                    _jsonDataCallBack.Invoke(downloadHandler.text);
                }
                else if (_Texture2DCallBack != null)
                {
                    //贴图回调
                    _Texture2DCallBack?.Invoke(DownloadHandlerTexture.GetContent(_webRequest));
                    GameGod.Instance.Log(E_Log.Proto, string.Format("<color=#FFF11A>{{\"code\":{0},\"data\":\"\"}}</color>", _webRequest.responseCode));
                }
                else if (_AudioClipCallBack != null)
                {
                    //音频回调
                    _AudioClipCallBack?.Invoke(DownloadHandlerAudioClip.GetContent(_webRequest));
                    GameGod.Instance.Log(E_Log.Proto, string.Format("<color=#FFF11A>{{\"code\":{0},\"data\":\"\"}}</color>", _webRequest.responseCode));
                }
                else
                {
                    //空回调，直接打印内容
                    GameGod.Instance.Log(E_Log.Proto, string.Format("<color=#FFF11A>{{\"code\":{0},\"data\":{1}}}</color>", _webRequest.responseCode, downloadHandler.text));
                }
            }
            //清理状态
            IsBusy = false;
            _jsonDataCallBack = null;
            _Texture2DCallBack = null;
            _currRetry = 0;
            _url = null;
            _webRequest.Dispose();
            //回池
            ThisPool.Recycle(this);
        }
    }
}