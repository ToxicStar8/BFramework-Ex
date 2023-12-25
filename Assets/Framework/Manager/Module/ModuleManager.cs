/*********************************************
 * BFramework
 * Module管理器
 * 对于网络游戏Module就是数据请求器以及数据缓存点，对于单机游戏Module就是数据的存档
 * 创建时间：2023/09/07 14:58:23
 *********************************************/
using LitJson;
using MainPackage;
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

        public void Init(Type[] typeArr)
        {
            //初始化Module
            for (int i = 0, length = typeArr.Length; i < length; i++)
            {
                var moduleType = typeArr[i];
                var moduleBase = Activator.CreateInstance(moduleType) as ModuleBase;
                moduleBase.OnInit();
                _allModuleDic[moduleType.Name] = moduleBase;
            }
        }

        /// <summary>
        /// 获取Module
        /// </summary>
        public T GetModule<T>() where T : ModuleBase
        {
            Type type = typeof(T);
            if (!_allModuleDic.ContainsKey(type.Name))
            {
                GameGod.Instance.Log(E_Log.Error, type.Name, "未进行初始化！");
                return null;
            }
            return _allModuleDic[type.Name] as T;
        }

        /// <summary>
        /// 保存Module数据到本地
        /// </summary>
        public void SaveModule<T>() where T : ModuleBase
        {
            Type type = typeof(T);
            var module = GetModule<T>();
            if (module != null)
            {
                File.WriteAllText(_savePath + type.Name, JsonMapper.ToJson(module));
                GameGod.Instance.Log(E_Log.Framework, type.Name, "保存成功");
            }
        }

        /// <summary>
        /// 读取本地数据到Module
        /// </summary>
        public void LoadModule<T>() where T : ModuleBase
        {
            Type type = typeof(T);
            var filePath = _savePath + type.Name;
            if(!File.Exists(filePath))
            {
                GameGod.Instance.Log(E_Log.Error, type.Name, "存档不存在");
                return;
            }

            var jsonData = File.ReadAllText(_savePath + type.Name);
            _allModuleDic[type.Name] = JsonMapper.ToObject<T>(jsonData);
            GameGod.Instance.Log(E_Log.Framework, type.Name, "加载成功");
        }

        public override void OnUpdate() { }
        public override void OnDispose() 
        {
            foreach (var item in _allModuleDic)
            {
                item.Value.OnDispose();
            }
            _allModuleDic.Clear();
            _allModuleDic = null;
        }
    }
}
