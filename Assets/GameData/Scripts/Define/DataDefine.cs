/*********************************************
 * BFramework
 * 数据定义
 * 创建时间：2023/04/23 09:53:23
 *********************************************/
using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameData
{
    public class  BaseData : GameBase
    {

    }

    /// <summary>
    /// 角色数据
    /// </summary>
    public class RoleData : BaseData
    {
        public int RoleId;          //角色Id
        public void SetRoleId(int cfgId) => RoleId = cfgId;
        public int HP;              //角色HP
        public void SetHP(int hp) => HP = hp;
    }
}
