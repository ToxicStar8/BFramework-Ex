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
    public class RedPointBase : GameBaseMono
    {
        //唯一标识符参数，用于区分Id多红点的情况
        [SerializeField]
        public string Key;

        //需要监听的红点Ids，在预制体上加
        [SerializeField]
        protected List<int> _redPointIds;

        // 当前红点数量
        protected Dictionary<int, int> _redPointNumDic;

        /// <summary>
        /// 当前红点数量
        /// </summary>
        protected int _curNum;

        public bool IsInit { get; private set; }

        private void Awake()
        {
            _redPointNumDic = new Dictionary<int, int>();
        }

        private void Start()
        {
            //先放到一个很远的地方，初始化完毕后再设回来
            var curPos = transform.localPosition;
            transform.localPosition = Vector3.one * 9999;

            Debug.Log("这里初始化红点");
            //添加所有需要监听的事件
            for (int i = 0; i < _redPointIds.Count; i++)
            {
                var id = _redPointIds[i];
                //添加所有需要监听的事件
                GameGod.Instance.RedPointManager.AddRedCallBack(id, UpdateRedPoint);
                //计算好的数量，存下来使用 
                var num1 = RedPointHelper.Instance.GetNumById(id, Key);
                if (num1 > 0)
                {
                    _curNum++;
                }
                _redPointNumDic.Add(id, num1);
            }
            ShowOrHideRed();

            IsInit = true;
            transform.localPosition = curPos;
        }

        /// <summary>
        /// 更新事件
        /// </summary>
        private void UpdateRedPoint(int id, string key, int num)
        {
            if (key == Key)
            {
                CalucateRedPoint(id, num);
                ShowOrHideRed();
            }
        }

        /// <summary>
        /// 重新计算红点
        /// </summary>
        private void CalucateRedPoint(int id, int num1)
        {
            // TD 显示
            if (_redPointNumDic.TryGetValue(id, out int value))
            {
                if (num1 > 0 && value == 0)
                {
                    value = 1;
                    _curNum++;
                }
                else if (num1 == 0 && value == 1)
                {
                    _redPointNumDic.Remove(id);
                    _curNum--;
                }
            }
            else
            {
                if (num1 > 0)
                {
                    _redPointNumDic.Add(id, 1);
                    _curNum++;
                }
            }
        }

        /// <summary>
        /// 显示红点
        /// </summary>
        protected virtual void ShowOrHideRed()
        {
            gameObject.SetActive(_curNum > 0);
        }

        public bool IsShow()
        {
            bool isShow = false;
            foreach (var item in _redPointNumDic)
            {
                isShow = item.Value > 0;
                if (isShow)
                {
                    break;
                }
            }
            return isShow;
        }

        private void OnDisable()
        {
            _curNum = 0;
            _redPointNumDic.Clear();
        }

        private void OnDestroy()
        {
            //移除所有监听
            for (int i = 0; i < _redPointIds.Count; i++)
            {
                var id = _redPointIds[i];
                GameGod.Instance.RedPointManager?.RemoveRedCallBack(id, UpdateRedPoint);
            }
        }
    }
}