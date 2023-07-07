/*********************************************
 * BFramework
 * 数据管理器
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using GameData;
using LitJson;
using MainPackage;
using System.IO;
using UnityEngine;

namespace GameData
{
    /// <summary>
    /// 数据列表
    /// </summary>
    public class AllData
    {
        /// <summary>
        /// 游玩总时间
        /// </summary>
        public float GamingTime;
        //直接在这里添加新的数据类型
        public PlayData PlayData;
        //统一回收
        public void OnDispose()
        {
            PlayData?.OnDispose();
        }
    }
}