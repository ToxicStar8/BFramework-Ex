/*********************************************
 * BFramework
 * 脚本名：ModuleBase.cs
 * 创建时间：2023/04/06 11:45:09
 *********************************************/
using GameData;
using System;

namespace Framework
{
    /// <summary>
    /// 数据操作基类
    /// </summary>
    public abstract class ModuleBase : GameBase
    {
        private EmtpyMono _emtpyMono;
        protected EmtpyMono EmtpyMono
        {
            get
            {
                _emtpyMono = _emtpyMono ?? new EmtpyMono();
                return _emtpyMono;
            }
        }
        public abstract void OnInit();
    }
}
