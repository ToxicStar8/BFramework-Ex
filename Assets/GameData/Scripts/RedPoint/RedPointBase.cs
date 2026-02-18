/*********************************************
 * 
 * 红点基类
 * 创建时间：2025/01/03 21:55:23
 *********************************************/
using Framework;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    /// <summary>
    /// 红点基类
    /// </summary>
    public abstract class RedPointBase : GameBaseMono
    {
        //唯一标识符参数，用于区分Id多红点的情况
        [SerializeField]
        public string Key;

        [SerializeField]
        protected List<int> RedPointIds;

        // 当前红点数量
        protected Dictionary<int, int> RedNumDict;

        /// <summary>
        /// 当前红点数量
        /// </summary>
        protected int CurNum;

        public bool IsInit { get; private set; }

        private void Awake()
        {
            this.RedNumDict = new Dictionary<int, int>();
        }

        private void Start()
        {
            //先放到一个很远的地方，初始化完毕后再设回来
            //var curPos = transform.localPosition;
            //transform.localPosition = new Vector3(9999, 9999, 9999);

            //Debug.Log("这里初始化红点");

            //添加所有需要监听的事件
            for (int i = 0; i < this.RedPointIds.Count; i++)
            {
                var id = this.RedPointIds[i];
                //添加所有需要监听的事件
                GameGod.Instance.RedPointManager.AddRedCallBack(id, this.UpdateRed);
                //计算好的 存下来使用 
                var num1 = RedPointHelper.Instance.GetNumById(id, Key);
                if (num1 > 0)
                {
                    this.CurNum++;
                }
                this.RedNumDict.Add(id, num1);
            }
            this.ShowOrHideRed();

            IsInit = true;
            //transform.localPosition = curPos;
        }

        /// <summary>
        /// 更新事件
        /// </summary>
        private void UpdateRed(int id, string key, int num)
        {
            if (key == Key)
            {
                this.CalucateRed(id, num);
                this.ShowOrHideRed();
            }
        }

        /// <summary>
        /// 重新计算红点
        /// </summary>
        private void CalucateRed(int id, int num1)
        {
            // TD 显示
            if (this.RedNumDict.TryGetValue(id, out int value))
            {
                if (num1 > 0 && value == 0)
                {
                    value = 1;
                    this.CurNum++;
                }
                else if (num1 == 0 && value == 1)
                {
                    this.RedNumDict.Remove(id);
                    this.CurNum--;
                }
            }
            else
            {
                if (num1 > 0)
                {
                    this.RedNumDict.Add(id, 1);
                    this.CurNum++;
                }
            }
        }

        /// <summary>
        /// 显示红点
        /// </summary>
        protected virtual void ShowOrHideRed()
        {
            this.SetActive(this.CurNum > 0);
            if (CurNum > 0)
                OnStartShowRed();
            else
                OnStopShowRed();
        }
        public virtual void OnStartShowRed() { }
        public virtual void OnStopShowRed() { }

        public bool IsShow()
        {
            bool isShow = false;
            foreach (var item in this.RedNumDict)
            {
                isShow = item.Value > 0;
                if (isShow)
                    break;
            }
            return isShow;
        }

        private void OnDisable()
        {
            this.CurNum = 0;
            this.RedNumDict.Clear();
        }

        private void OnDestroy()
        {
            //移除所有监听
            for (int i = 0; i < this.RedPointIds.Count; i++)
            {
                var id = this.RedPointIds[i];
                GameGod.Instance.RedPointManager?.RemoveRedCallBack(id, this.UpdateRed);
            }

            this.RedNumDict.Clear();
            this.RedPointIds.Clear();
        }
    }
}