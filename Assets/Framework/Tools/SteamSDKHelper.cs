#if UNITY_STANDALONE_WIN
/*********************************************
 * BFramework
 * SteamSDK工具类
 * 创建时间：2023/12/27 17:43:00
 *********************************************/
using MainPackage;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Framework
{
    /// <summary>
    /// SteamSDK工具类
    /// </summary>
    public class SteamSDKHelper : GameBase
    {
        /// <summary>
        /// 创建大厅回调
        /// </summary>
        private Callback<LobbyCreated_t> _lobbyCreatedCallback;
        /// <summary>
        /// 从大厅加入游戏的回调
        /// </summary>
        private Callback<GameLobbyJoinRequested_t> _gameLobbyJoinRequestedCallback;
        /// <summary>
        /// 加入大厅后的回调
        /// </summary>
        private Callback<LobbyEnter_t> _lobbyEnterCallback;

        private const string _hostAddressKey = "HostAddress";
        /// <summary>
        /// WebSocket Url
        /// </summary>
        private string _hostAddressValue;
        private Action _openCallback;
        private Action _closeCallback;

        public SteamSDKHelper(Action OpenCallback, Action CloseCallback)
        {
            _openCallback = OpenCallback;
            _closeCallback = CloseCallback;

            InitSteamCallback();
        }

        #region Callback
        /// <summary>
        /// 初始化Steam监听回调
        /// </summary>
        private void InitSteamCallback()
        {
            _lobbyCreatedCallback = Callback<LobbyCreated_t>.Create(LobbyCreatedCallback);
            _gameLobbyJoinRequestedCallback = Callback<GameLobbyJoinRequested_t>.Create(GameLobbyJoinRequestedCallback);
            _lobbyEnterCallback = Callback<LobbyEnter_t>.Create(LobbyEnterCallback);
        }

        /// <summary>
        /// 创建大厅回调
        /// </summary>
        private void LobbyCreatedCallback(LobbyCreated_t result)
        {
            if (result.m_eResult != EResult.k_EResultOK)
            {
                LogError("Steam大厅创建失败！");
                return;
            }
            Log("Steam大厅创建成功！");

            //todo:这里写服务器启动


            //设置大厅数据
            SteamMatchmaking.SetLobbyData(new CSteamID(result.m_ulSteamIDLobby), _hostAddressKey, _hostAddressValue);
        }

        /// <summary>
        /// 从好友列表中加入大厅时调用
        /// </summary>
        private void GameLobbyJoinRequestedCallback(GameLobbyJoinRequested_t result)
        {
            SteamMatchmaking.JoinLobby(result.m_steamIDLobby);
            Log("Steam好友尝试从好友列表加入！");
        }

        /// <summary>
        /// 加入大厅后的回调
        /// </summary>
        private void LobbyEnterCallback(LobbyEnter_t result)
        {
            //如果是自己开的房间，就不连接（先不拦
            //if (!string.IsNullOrWhiteSpace(_hostAddressValue))
            //{
            //    return;
            //}

            string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(result.m_ulSteamIDLobby), _hostAddressKey);
            SocketConnect(hostAddress, _openCallback, _closeCallback);
        }

        #endregion

        /// <summary>
        /// 创建大厅
        /// </summary>
        public void CreateLobby(string wsUrl)
        {
            if (!SteamManager.Initialized)
            {
                LogError("Steam未初始化");
                return;
            }
            //Log(SteamFriends.GetPersonaName());
            _hostAddressValue = wsUrl;
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, ConstDefine.RoomMaxPeople);
        }
    }
}
#endif