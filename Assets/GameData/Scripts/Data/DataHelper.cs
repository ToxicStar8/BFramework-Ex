/*********************************************
 * BFramework
 * 脚本名：DataHelper.cs
 * 创建时间：2023/05/05 14:08:09
 *********************************************/
using Framework;

namespace GameData
{
    /// <summary>
    /// Data数据快捷访问
    /// </summary>
    public static class DataHelper
    {
        //配表快捷访问
        //public static TableGlobalCtrl TbGlobalCtrl => GameGod.Instance.TableManager.GetTableCtrl<TableGlobalCtrl>();



        //数据快捷访问
        public static PlayData PlayData => GameGod.Instance.DataManager.AllData.PlayData;
    }
}