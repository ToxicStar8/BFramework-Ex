/*********************************************
 * BFramework
 * Socket管理器
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Socket管理器
    /// </summary>
    public class SocketManager : ManagerBase
    {
        private SocketRoutine _mainSocket;

        public override void OnStart()
        {
            _mainSocket = new SocketRoutine();
            _mainSocket.OnInit(ConstDefine.ServerWS);
        }

        /// <summary>
        /// 连接Socket
        /// </summary>
        public void Connect(Action callback = null)
        {
            _mainSocket.Connect(callback);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public void SendMsg(string msg, Action<string> callBack = null)
        {
            _mainSocket.SendMsg(msg);
        }

        /// <summary>
        /// 关闭Socket
        /// </summary>
        public void CloseSocket()
        {
            _mainSocket.CloseSocket();
        }

        public override void OnUpdate() 
        {
            _mainSocket?.OnUpdate();
        }

        public override void OnDispose() 
        {
            _mainSocket.CloseSocket();
            _mainSocket = null;
        }
    }
}
