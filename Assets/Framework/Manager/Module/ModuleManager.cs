/*********************************************
 * BFramework
 * Module管理器
 * 对于网络游戏Module就是数据请求器以及数据缓存点，对于单机游戏Module就是数据的存档
 * 创建时间：2023/09/07 14:58:23
 *********************************************/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Module管理器
    /// </summary>
    public class ModuleManager : ManagerBase
    {
        /// <summary>
        /// 全部的Module类型
        /// </summary>
        private Type[] _allModuleType;

        /// <summary>
        /// 全部Module的字典
        /// </summary>
        private Dictionary<string, ModuleBase> _allModuleDic;

        /// <summary>
        /// Module的存储地址
        /// </summary>
        private string _savePath;

        public override void OnInit()
        {
            _allModuleDic = new Dictionary<string, ModuleBase>();
            _savePath = Application.persistentDataPath + "/";
        }

        /// <summary>
        /// 初始化存档类型
        /// </summary>
        public void InitModuleType(Type[] typeArr)
        {
            _allModuleType = typeArr;
        }

        /// <summary>
        /// New全部的Module
        /// </summary>
        public void NewAllModule()
        {
            //初始化Module
            for (int i = 0, length = _allModuleType.Length; i < length; i++)
            {
                var moduleType = _allModuleType[i];
                var moduleBase = Activator.CreateInstance(moduleType) as ModuleBase;
                moduleBase.OnNew();
                _allModuleDic[moduleType.Name] = moduleBase;
            }
        }

        /// <summary>
        /// 获取Module
        /// </summary>
        public T GetModule<T>() where T : ModuleBase
        {
            Type type = typeof(T);
            return GetModuleBase(type) as T;
        }

        /// <summary>
        /// 获取ModuleBase
        /// </summary>
        public ModuleBase GetModuleBase(Type type)
        {
            if (!_allModuleDic.ContainsKey(type.Name))
            {
                GameGod.Instance.Log(E_Log.Error, "未初始化Module", type.Name);
                return null;
            }
            return _allModuleDic[type.Name];
        }

        /// <summary>
        /// 获取任意的Module是否为空
        /// </summary>
        public bool GetAnyModulePathIsNull()
        {
            for (int i = 0, length = _allModuleType.Length; i < length; i++)
            {
                var type = _allModuleType[i];
                bool isNull = GetModulePathIsNull(type);
                if (isNull)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取指定的Module是否有存档在本地
        /// </summary>
        public bool GetModulePathIsNull<T>()
        {
            Type type = typeof(T);
            return GetModulePathIsNull(type);
        }

        /// <summary>
        /// 获取指定的Module是否有存档在本地
        /// </summary>
        private bool GetModulePathIsNull(Type type)
        {
            var filePath = _savePath + type.Name;
            if (!File.Exists(filePath))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 保存全部Module数据到本地
        /// </summary>
        public void SaveAllModule()
        {
            for (int i = 0, length = _allModuleType.Length; i < length; i++)
            {
                var type = _allModuleType[i];
                SaveModule(type);
            }
        }

        /// <summary>
        /// 保存Module数据到本地
        /// </summary>
        public void SaveModule<T>() where T : ModuleBase
        {
            Type type = typeof(T);
            SaveModule(type);
        }

        private void SaveModule(Type type)
        {
            var module = GetModuleBase(type);
            if (module != null)
            {
                File.WriteAllText(_savePath + type.Name, JsonConvert.SerializeObject(module));
                GameGod.Instance.Log(E_Log.Framework, "保存Module", type.Name);
            }
        }

        /// <summary>
        /// 读取本地全部数据到Module
        /// </summary>
        public void LoadAllModule()
        {
            for (int i = 0, length = _allModuleType.Length; i < length; i++)
            {
                var type = _allModuleType[i];
                LoadModule(type);
            }
        }

        /// <summary>
        /// 读取本地数据到Module
        /// </summary>
        public void LoadModule<T>() where T : ModuleBase
        {
            Type type = typeof(T);
            LoadModule(type);
        }

        /// <summary>
        /// 读取本地数据到Module
        /// </summary>
        private void LoadModule(Type type)
        {
            var filePath = _savePath + type.Name;
            if (!File.Exists(filePath))
            {
                GameGod.Instance.Log(E_Log.Error, type.Name, "存档不存在");
                return;
            }

            var jsonData = File.ReadAllText(_savePath + type.Name);
            var obj = JsonConvert.DeserializeObject(jsonData, type);
            var moduleBase = obj as ModuleBase;
            moduleBase.OnLoad();
            _allModuleDic[type.Name] = moduleBase;
            GameGod.Instance.Log(E_Log.Framework, "加载Module", type.Name);
        }

        /// <summary>
        /// 注销全部Module
        /// </summary>
        public void CloseAllModule()
        {
            if (_allModuleDic is not null)
            {
                foreach (var item in _allModuleDic)
                {
                    item.Value.OnDispose();
                }
                _allModuleDic.Clear();
            }
        }

        public override void OnUpdate() { }
        public override void OnDispose()
        {
            CloseAllModule();
            _allModuleDic = null;
        }
    }
}
