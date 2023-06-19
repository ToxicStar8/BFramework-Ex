/*********************************************
 * BFramework
 * 脚本名：ModuleBase.cs
 * 创建时间：2023/04/06 11:45:09
 *********************************************/
using Framework;

namespace GameData
{
    /// <summary>
    /// 数据操作基类
    /// </summary>
    public abstract class ModuleBase : GameBase
    {
        //配表快捷访问
        public static TableRoleCtrl TbRoleCtrl => GameGod.Instance.TableManager.GetTableCtrl<TableRoleCtrl>();
        public static TableSuitCardCtrl TbSuitCardCtrl => GameGod.Instance.TableManager.GetTableCtrl<TableSuitCardCtrl>();

        //数据快捷访问
        public static PlayData PlayData => GameGod.Instance.DataManager.PlayData;
    }
}
