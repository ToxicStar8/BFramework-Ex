/*********************************************
 * BFramework
 * 热更入口
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using Framework;
using HybridCLR;
using MainPackage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    /// <summary>
    /// 挂载脚本加载热更
    /// </summary>
    public class HotUpdateMain : MonoBehaviour
    {
        void Start()
        {
            GameEntry.Instance.Log(E_Log.Framework, "热更代码", "进入成功");
            //初始化游戏总控制器
            var gameGod = new GameObject("GameGod");
            gameGod.AddComponent<GameGod>();
            //Log
            gameGod.AddComponent<Log>();
            //初始化表格
            GameGod.Instance.TableManager.Init(TableTypes.TableCtrlTypeArr);
            //背景音乐
            GameGod.Instance.AudioManager.PlayBackground("RetroComedy.ogg");
            //游戏管理器
            GameManager.CreateGameManager();
            //正式启动
            GameGod.Instance.UIManager.OpenUI<UIMainMenu>(E_UILevel.Common);
        }
    }

    /// <summary>
    /// 反射加载热更（已弃用）
    /// </summary>
    //public class HotUpdateMainByMethod
    //{
    //    public static void Start()
    //    {
    //          
    //    }
    //}
}
