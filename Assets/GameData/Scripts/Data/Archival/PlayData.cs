/*********************************************
 * 
 * 玩家数据
 * 创建时间：2023/01/29 15:30:23
 *********************************************/
using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameData
{
    /// <summary>
    /// 玩家数据
    /// </summary>
    public class PlayData : DataBase
    {
        public override void OnInit()
        {
            //初始化角色数据
            RoleData = new RoleData();
        }
        public RoleData RoleData { private set; get; }
        public override void OnDispose()
        {

        }
    }
}
