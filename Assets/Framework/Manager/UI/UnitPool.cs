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
        /// 自己作为模板
        /// </summary>
        private T _unitBase;

        /// <summary>
        /// UI的加载器
        /// </summary>
        private LoadHelper LoadHelper;

        /// <summary>
        /// 池队列
        /// </summary>
        private Queue<T> PoolQueue = new Queue<T>();

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
        public UnitPool(T go, LoadHelper loadHelper, RectTransform rootRect, bool isShowGo = false)
        {
            LoadHelper = loadHelper;
            _rootRect = rootRect;
            _unitBase = go;
            _unitBase.SetActive(isShowGo);
            GameGod.Instance.Log(E_Log.Log, "创建对象池", UnitName);
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
                unitBase = Object.Instantiate(_unitBase, parent).GetComponent<T>();
                unitBase.LoadHelper = LoadHelper;
                unitBase.OnAwake();
            }
            else
            {
                //GameEntry.Instance.Log(E_Log.Framework, "已有" + UnitName + "对象", "取出");
                unitBase = PoolQueue.Dequeue();
                unitBase.gameObject.SetParent(parent);
            }
            PoolList.Add(unitBase);
            unitBase.rectTransform.localPosition = _unitBase.transform.localPosition;
            unitBase.rectTransform.localRotation = _unitBase.transform.localRotation;
            unitBase.rectTransform.localScale = _unitBase.transform.localScale;
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
            obj.OnRecycle();
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
                obj.OnRecycle();
                obj.gameObject.SetParent(_rootRect);
                obj.gameObject.SetActive(false);
                PoolQueue.Enqueue(obj);
            }
            PoolList.Clear();
        }

        /// <summary>
        /// 销毁对象
        /// </summary>
        public void Release(T obj)
        {
            obj.OnBeforeDestroy();
            obj.gameObject.Destroy();
            PoolList.Remove(obj);
            obj = null;
        }

        /// <summary>
        /// 只销毁已创建出来的对象
        /// </summary>
        public void ReleaseAll()
        {
            for (int i = 0; i < PoolList.Count; i++)
            {
                var obj = PoolList[i];
                obj.OnBeforeDestroy();
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
                _unitBase.LoadHelper = LoadHelper;
                _unitBase.OnAwake();
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
    }
}