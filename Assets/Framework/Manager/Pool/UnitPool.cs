/*********************************************
 * BFramework
 * Unit对象池
 * 创建时间：2023/01/08 20:40:23
 *********************************************/
using MainPackage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Unit对象池
    /// </summary>
    public class UnitPool<T> where T : UnitBase, new()
    {
        /// <summary>
        /// 所属根节点
        /// </summary>
        private RectTransform _rootRect;

        /// <summary>
        /// 游戏对象模板
        /// </summary>
        private GameObject gameObject;

        /// <summary>
        /// UI根节点
        /// </summary>
        private UIBase _uiParent;

        /// <summary>
        /// Unit根节点 与UIParent相斥 二选一
        /// </summary>
        private UnitBase _unitParent;

        /// <summary>
        /// 自己的模板对象
        /// </summary>
        private T _unitBase;

        /// <summary>
        /// UI的加载器
        /// </summary>
        public LoadHelper LoadHelper { private set; get; }

        /// <summary>
        /// 池队列
        /// </summary>
        public Queue<T> PoolQueue { private set; get; } = new Queue<T>();

        /// <summary>
        /// 已创建出来的对象
        /// </summary>
        public List<T> PoolList { private set; get; } = new List<T>();

        public string UnitName => typeof(T).Name;
        public int Count => PoolList.Count;
        public T this[int index] => PoolList[index];

        /// <summary>
        /// 初始化Unit池
        /// </summary>
        /// <param name="uiBase">父UI节点</param>
        /// <param name="go">需要克隆的对象</param>
        /// <param name="isShowGo">是否显示原游戏对象</param>
        public UnitPool(UIBase uiBase, GameObject go, bool isShowGo = false)
        {
            _uiParent = uiBase;
            LoadHelper = uiBase.LoadHelper;
            _rootRect = uiBase.rectTransform;
            gameObject = go;
            gameObject.SetActive(isShowGo);
        }

        /// <summary>
        /// 初始化Unit池
        /// </summary>
        /// <param name="unitBase">父Unit节点</param>
        /// <param name="go">需要克隆的对象</param>
        /// <param name="isShowGo">是否显示原游戏对象</param>
        public UnitPool(UnitBase unitBase, GameObject go, bool isShowGo = false)
        {
            _unitParent = unitBase;
            LoadHelper = unitBase.LoadHelper;
            _rootRect = unitBase.rectTransform;
            gameObject = go;
            gameObject.SetActive(isShowGo);
        }

        /// <summary>
        /// 创建为Unit对象
        /// </summary>
        public T CreateUnit(RectTransform parent)
        {
            T unitBase = null;
            if (PoolQueue.Count == 0)
            {
                //GameEntry.Instance.Log(E_Log.Framework, "不存在" + UnitName + "对象", "创建");
                unitBase = new T();
                //初始化
                if (_uiParent != null)
                    unitBase.UIParent = _uiParent;
                else if (_unitParent != null)
                    unitBase.UnitParent = _unitParent;
                unitBase.gameObject = Object.Instantiate(gameObject, parent);
                unitBase.LoadHelper = LoadHelper;
                unitBase.OnCreate();
                unitBase.OnInit();
            }
            else
            {
                //GameEntry.Instance.Log(E_Log.Framework, "已有" + UnitName + "对象", "取出");
                unitBase = PoolQueue.Dequeue();
                unitBase.gameObject.SetParent(parent);
            }
            PoolList.Add(unitBase);
            unitBase.rectTransform.localPosition = gameObject.transform.localPosition;
            unitBase.rectTransform.localRotation = gameObject.transform.localRotation;
            unitBase.rectTransform.localScale = gameObject.transform.localScale;
            unitBase.gameObject.SetActive(true);
            return unitBase;
        }

        #region LoopScrollRect
        /// <summary>
        /// 为LoopScrollRect服务 直接返回RectTransform
        /// </summary>
        public RectTransform CreateUnitToRect(RectTransform parent)
        {
            var  unitBase = CreateUnit(parent);
            return unitBase.rectTransform;
        }

        /// <summary>
        /// 获取指定unit
        /// </summary>
        public T Find(RectTransform rt)
        {
            T unit = null;
            for (int i = 0; i < PoolList.Count; i++)
            {
                unit = PoolList[i];
                if (unit.rectTransform == rt)
                {
                    return unit;
                }
            }
            return null;
        }
        #endregion

        /// <summary>
        /// 回收对象
        /// </summary>
        public void Recycle(T obj)
        {
            obj.gameObject.SetParent(_rootRect);
            obj.gameObject.SetActive(false);
            PoolQueue.Enqueue(obj);
            PoolList.Remove(obj);
        }

        /// <summary>
        /// 回收全部对象
        /// </summary>
        public void RecycleAll()
        {
            for (int i = 0; i < PoolList.Count; i++)
            {
                var obj = PoolList[i];
                obj.gameObject.SetParent(_rootRect);
                obj.gameObject.SetActive(false);
                PoolQueue.Enqueue(obj);
            }
            PoolList.Clear();
        }

        /// <summary>
        /// 销毁对象
        /// </summary>
        public void Relase(T obj)
        {
            obj.gameObject.Destroy();
            PoolList.Remove(obj);
            obj = null;
        }

        /// <summary>
        /// 只销毁已创建出来的对象
        /// </summary>
        public void RelaseAll()
        {
            for (int i = 0; i < PoolList.Count; i++)
            {
                var obj = PoolList[i];
                obj.gameObject.Destroy();
                obj = null;
            }
            PoolList.Clear();
        }

        /// <summary>
        /// 创建为Panel对象
        /// 注：不可回收、销毁，跟随UI一起销毁！
        /// </summary>
        public T CreateBase()
        {
            if (_unitBase == null)
            {
                GameGod.Instance.Log(E_Log.Framework, "Panel" + UnitName, "创建");
                _unitBase = new T();
                _unitBase.gameObject = gameObject;
                _unitBase.LoadHelper = LoadHelper;
                _unitBase.OnCreate();
                _unitBase.OnInit();
            }
            _unitBase.gameObject.SetActive(true);
            return _unitBase;
        }

        /// <summary>
        /// 直接获取Panel
        /// </summary>
        public T GetUnitBase()
        {
            if (_unitBase == null)
            {
                CreateBase();
            }
            return _unitBase;
        }

        /// <summary>
        /// 隐藏Panel
        /// </summary>
        public void HideUnitBase()
        {
            if (_unitBase == null)
            {
                GameGod.Instance.Log(E_Log.Error, UnitName, "的Panel不存在！");
                return;
            }
            _unitBase.gameObject.SetActive(false);
        }
    }
}