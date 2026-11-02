/*********************************************
 * BFramework
 * UI管理器
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using MainPackage;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// UI管理器
    /// </summary>
    public class UIManager : ManagerBase
    {
        /// <summary>
        /// 已打开的UI字典
        /// </summary>
        private Dictionary<string, UIBase> _uiBaseDic;

        /// <summary>
        /// UI Prefab 加载器
        /// </summary>
        private LoadHelper _loadHelper;

        public override void OnAwake()
        {
            _uiBaseDic = new Dictionary<string, UIBase>();
            _loadHelper = LoadHelper.Create();
        }

        /// <summary>
        /// 打开UI
        /// </summary>
        public void OpenUI<T>(E_UILevel uiLevel, params object[] args) where T : UIBase,new()
        {
            var uiName = typeof(T).Name;
            OpenUI<T>(uiName,uiLevel, args);
        }

        /// <summary>
        /// 打开UI
        /// </summary>
        public void OpenUI<T>(string uiName, E_UILevel uiLevel, params object[] args) where T : UIBase, new()
        {
            //已打开 直接显示
            if (_uiBaseDic.TryGetValue(uiName, out var uiBase))
            {
                uiBase.gameObject.SetActive(true);
                uiBase.OnShow(args);
                return;
            }

            var uiTrans = GameGod.Instance.GetUILevelTrans(uiLevel);
            var uiObj = _loadHelper.CreateGameObjectSync(uiName, uiTrans);
            if (uiObj == null)
            {
                _loadHelper.Unload(uiName, destroyInstances: true);
                return;
            }

            uiBase = uiObj.GetComponent<T>();
            if (uiBase == null)
            {
                GameGod.Instance.Log(E_Log.Error, uiName, "未找到UI组件");
                _loadHelper.Unload(uiName, destroyInstances: true);
                return;
            }

            uiBase.UIName = uiName;
            uiBase.LoadHelper = LoadHelper.Create();
            uiBase.OnAwake();
            uiBase.OnShow(args);
            _uiBaseDic[uiName] = uiBase;
        }

        /// <summary>
        /// 获取UI
        /// </summary>
        public T GetUI<T>() where T : UIBase
        {
            var uiName = typeof(T).Name;
            return GetUI(uiName) as T;
        }

        /// <summary>
        /// 获取UI
        /// </summary>
        public UIBase GetUI(string uiName)
        {
            if (!_uiBaseDic.TryGetValue(uiName, out var uiBase))
            {
                return null;
            }
            return uiBase;
        }

        /// <summary>
        /// 隐藏UI
        /// </summary>
        public void HideUI<T>() where T : UIBase
        {
            var uiName = typeof(T).Name;
            HideUI(uiName);
        }

        /// <summary>
        /// 隐藏UI
        /// </summary>
        public void HideUI(string uiName)
        {
            //已打开 直接关闭
            if (_uiBaseDic.TryGetValue(uiName, out var uiBase))
            {
                uiBase.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        public void CloseUI<T>() where T : UIBase
        {
            var uiName = typeof(T).Name;
            CloseUI(uiName);
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        public void CloseUI(string uiName)
        {
            //已打开 直接关闭
            if (_uiBaseDic.TryGetValue(uiName, out var uiBase))
            {
                uiBase.OnDispose();
                uiBase = null;
                _uiBaseDic.Remove(uiName);
                _loadHelper.Unload(uiName, destroyInstances: true);
            }
        }

        /// <summary>
        /// 关闭全部UI
        /// </summary>
        public void CloseAll()
        {
            foreach (var item in _uiBaseDic)
            {
                item.Value.OnDispose();
                _loadHelper.Unload(item.Key, destroyInstances: true);
            }
            _uiBaseDic.Clear();
        }

        public override void OnUpdate() { }
        public override void OnDispose()
        {
            CloseAll();
            _uiBaseDic = null;
            LoadHelper.Recycle(_loadHelper);
            _loadHelper = null;
        }
    }
}
