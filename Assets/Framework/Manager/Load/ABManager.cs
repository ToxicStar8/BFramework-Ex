/*********************************************
 * BFramework
 * AB包管理器
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using LitJson;
using MainPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Framework
{
    public class ABManager : ManagerBase
    {
        /// <summary>
        /// AB包信息
        /// </summary>
        public ABInfo ABInfo { private set; get; }

        public override void OnStart()
        {
            if (GameEntry.Instance.IsEditorMode)
            {
                if (GameEntry.Instance.IsRunABPackage)
                {
                    EditorLoadABPackage();
                }
                else
                {
                    GameEntry.Instance.WinLoading.IsInitEnd = true;
                }
            }
            else
            {

            }
        }

        /// <summary>
        /// 编辑器模式加载AB包
        /// </summary>
        private void EditorLoadABPackage()
        {
            //编辑器模式下直接加载AB包索引信息
            var abPackage = AssetBundle.LoadFromFile(GameEntry.Instance.DowloadManager.SavePath + ConstDefine.ABInfoName);
            if(abPackage == null)
            {
                GameEntry.Instance.Log(E_Log.Error, "没有找到AB包！");
                return;
            }
            var textAsset = abPackage.LoadAsset<TextAsset>(ConstDefine.ABInfoName + ".json");
            //转化为ABInfo
            ABInfo = JsonMapper.ToObject<ABInfo>(textAsset.text);
            abPackage.Unload(true);
            GameEntry.Instance.Log(E_Log.Framework, "索引加载完毕");
        }

        public override void OnUpdate() { }
        public override void OnDispose() { }
    }
}
