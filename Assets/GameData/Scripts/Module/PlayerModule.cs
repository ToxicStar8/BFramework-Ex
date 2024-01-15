/*********************************************
 * 
 * 脚本名：StorageModule.cs
 * 创建时间：2023/04/06 11:45:09
 *********************************************/
using Framework;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    /// <summary>
    /// 玩家数据操作
    /// </summary>
    public class PlayerModule : ModuleBase
    {
        public static PlayerModule GetModule() => GameGod.Instance.ModuleManager.GetModule<PlayerModule>();

        public override void OnInit()
        {
            //这里添加监听 todo：跟UI一样Dispose就注销监听
            //AddEventListener((ushort)ModuleEvent.ModuleTest, ModuleTest);
        }

        public override void OnLoad()
        {

        }

        /// <summary>
        /// 登录至服务器
        /// </summary>
        public void C2S_Login(string access, string password, Action callback)
        {
            string url = Framework.ConstDefine.ServerHttp + "api/login";
            var sendMsg = new { access, password };
            var json = JsonMapper.ToJson(sendMsg);
            HttpPost(url, json, jsonData =>
            {
                callback?.Invoke();
            });
        }

        public void C2S_ConnentSocket(Action callback)
        {
            SocketConnect(Framework.ConstDefine.ServerWS, () => {
                callback?.Invoke();
            });
        }

        public override void OnBeforeDispose()
        {

        }
    }
}
