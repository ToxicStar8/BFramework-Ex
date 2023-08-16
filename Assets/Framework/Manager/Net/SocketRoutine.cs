﻿/*********************************************
 * BFramework
 * Socket访问器
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using UnityWebSocket;
using MainPackage;
using System.IO;
using System.IO.Compression;

namespace Framework
{
    /// <summary>
    /// Socket访问器
    /// </summary>
    public class SocketRoutine
    {
        /// <summary>
        /// Socket管理器
        /// </summary>
        public SocketManager SoctetMgr => GameGod.Instance.SocketManager;

        /// <summary>
        /// 客户端Socket
        /// </summary>
        public WebSocket Socket;

        /// <summary>
        /// 拦截的消息队列
        /// </summary>
        private Queue<SocketEvent> _eventQueue;

        private string _url;       //当前需要链接的地址

        /// <summary>
        /// 开启回调
        /// </summary>
        private Action OpenCallback;

        public void OnInit(string url)
        {
            _url = url;

            Socket = new WebSocket(_url);
            _eventQueue = new Queue<SocketEvent>();

            Socket.OnMessage += (sender, e) =>
            {
                lock (_eventQueue)
                {
                    _eventQueue.Enqueue(new SocketEvent(2, sender, e.Data, e.RawData));
                }
            };
            Socket.OnOpen += (sender, e) =>
            {
                lock (_eventQueue)
                {
                    _eventQueue.Enqueue(new SocketEvent(4, sender));
                }
            };
            Socket.OnError += (sender, e) =>
            {
                lock (_eventQueue)
                {
                    _eventQueue.Enqueue(new SocketEvent(1, sender, e.Message));
                }
            };
            Socket.OnClose += (sender, e) =>
            {
                lock (_eventQueue)
                {
                    var evt = new SocketEvent(3, sender);
                    evt.code = e.Code;
                    evt.reason = e.Reason;
                    _eventQueue.Enqueue(evt);
                }
            };
        }

        /// <summary>
        /// 连接
        /// </summary>
        public void Connect(Action callback = null, Dictionary<string, string> headerDic = null)
        {
            OpenCallback = callback;
            Socket.ConnectAsync(headerDic);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        public void SendMsg(string msg)
        {
            GameGod.Instance.Log(E_Log.Proto, "WebSocket 发送消息", msg);
            Socket.SendAsync(msg);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        public void SendMsg(byte[] msg)
        {
            GameGod.Instance.Log(E_Log.Proto, "WebSocket 发送消息", msg.ToString());
            Socket.SendAsync(msg);
        }

        /// <summary>
        /// 获取SocketClient
        /// </summary>
        public System.Net.WebSockets.ClientWebSocket GetSocket()
        {
            return Socket.GetSocket();
        }

        /// <summary>
        /// 关闭Socket
        /// </summary>
        /// <param name="msg"></param>
        public void CloseSocket()
        {
            Socket.CloseAsync();
        }

        public void OnUpdate()
        {
            lock (_eventQueue)
            {
                while (_eventQueue.TryDequeue(out SocketEvent evt))
                {
                    RcvSocketMs(evt);
                }
            }
        }

        /// <summary>
        /// 接收解析WebSocket消息
        /// </summary>
        /// <param name="evt"></param>
        private void RcvSocketMs(SocketEvent evt)
        {
            switch (evt.type)
            {
                case 1:         // 错误
                    GameGod.Instance.Log(E_Log.Error, "WebSocket 错误", evt.msg);
                    break;
                case 2:         // 消息
                    GameGod.Instance.Log(E_Log.Proto, "WebSocket 接收消息", evt.msg);
                    //分发消息
                    DispatchMsg(evt);
                    break;
                case 3:         // WS 关闭
                    GameGod.Instance.Log(E_Log.Proto, "WebSocket 主动关闭");
                    break;
                case 4:         // WS 打开
                    GameGod.Instance.Log(E_Log.Proto, "WebSocket 已连接");
                    OpenCallback?.Invoke();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 派送消息
        /// </summary>
        /// <param name="msg"></param>
        private void DispatchMsg(SocketEvent evt)
        {
            //根据实际项目情况修改
            var msg = evt.msg;
            var jsonData = JsonMapper.ToObject(msg);
            Debug.Log(jsonData.ToString());
        }
    }

    /// <summary>
    /// Socket事件类
    /// </summary>
    public class SocketEvent
    {
        public int type;    //握手次数 1掉线 2消息 3主动关闭 4连接
        public object ws;
        public string msg;
        //错误码和原因
        public ushort code;
        public string reason;
        public byte[] bytes;

        public SocketEvent(int type, object ws, string msg = null, byte[] bytes = null)
        {
            this.type = type;
            this.ws = ws;
            this.msg = msg;
            this.bytes = bytes;
        }
    }
}