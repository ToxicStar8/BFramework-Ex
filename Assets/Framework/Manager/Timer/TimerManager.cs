/*********************************************
 * BFramework
 * 计时器管理器
 * 创建时间：2023/01/30 11:12:23
 *********************************************/
using Cysharp.Threading.Tasks;
using MainPackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Framework
{
    /// <summary>
    /// 定时器信息类
    /// </summary>
    public class TimerInfo
    {
        public Action Callback;             //执行回调
        public int AllCount;                //执行次数
        public int InviteTime;              //执行间隔时间，毫秒
        public bool IsExecImmed;            //是否立即执行
        public CancellationTokenSource Cts; //取消用唯一Key
        //定时器管理器里赋值
        public string TimerName;            //定时器名

        public static TimerInfo Create(int allCount, int inviteTime, bool isExecImmed, Action callback)
        {
            var timerInfo = GameGod.Instance.PoolManager.CreateClassObj<TimerInfo>();
            timerInfo.AllCount = allCount;
            timerInfo.InviteTime = inviteTime;
            timerInfo.IsExecImmed = isExecImmed;
            timerInfo.Callback = callback;
            timerInfo.Cts = new CancellationTokenSource();
            return timerInfo;
        }
        public static void Recycle(TimerInfo timerInfo)
        {
            timerInfo.Cts = null;
            timerInfo.Callback = null;
            GameGod.Instance.PoolManager.RecycleClassObj(timerInfo);
        }
    }

    /// <summary>
    /// 定时器管理器
    /// </summary>
    public class TimerManager : ManagerBase
    {
        /// <summary>
        /// 定时器字典
        /// </summary>
        private Dictionary<string, TimerInfo> _timerInfoDic;

#if !UNITY_WEBGL
        /// <summary>
        /// 等待开始的列表
        /// </summary>
        private List<TimerInfo> _waitStartList;
#endif

#if UNITY_EDITOR
        public Dictionary<string, TimerInfo> TimerInfoDic
        {
            get { return _timerInfoDic; }
        }
#endif

        /// <summary>
        /// 临时Key
        /// </summary>
        private int _tempIndex;

        public override void OnInit() 
        {
            _timerInfoDic = new Dictionary<string, TimerInfo>();

            //WEBGL不支持多线程
#if !UNITY_WEBGL
            _waitStartList = new List<TimerInfo>();
            //初始化的时候启动一个线程专门处理计时器
            UniTask.RunOnThreadPool(Loop);
#endif
        }

#if !UNITY_WEBGL
        private async UniTask Loop()
        {
            while (true)
            {
                for (int i = 0; i < _waitStartList.Count; i++)
                {
                    ExecTimer(_waitStartList[i]).Forget();
                }
                _waitStartList.Clear();
                await UniTask.Yield();
            }
        }
#endif

        /// <summary>
        /// 添加一次性定时器
        /// </summary>
        public void AddTempTimer(TimerInfo timerInfo)
        {
            AddTimer(_tempIndex.ToString(), timerInfo);
            _tempIndex++;
        }

        /// <summary>
        /// 添加定时器
        /// </summary>
        public void AddTimer(string timerName,TimerInfo timerInfo)
        {
            if (timerInfo.AllCount <= 0)
            {
                GameGod.Instance.Log(E_Log.Error, "计时器的执行次数永远不能小于等于0！");
                return;
            }

            GameGod.Instance.Log(E_Log.Framework, "定时器添加", timerName);
            //记录名字
            timerInfo.TimerName = timerName;
            //添加到计时器的字典里
            _timerInfoDic.Add(timerName, timerInfo);
            //添加到等待队列里
#if !UNITY_WEBGL
            _waitStartList.Add(timerInfo);
#else
            ExecTimer(timerInfo);
#endif
        }

        /// <summary>
        /// 执行定时器
        /// </summary>
        private async UniTaskVoid ExecTimer(TimerInfo timerInfo)
        {
            //判断是否马上执行
            if (timerInfo.IsExecImmed)
            {
                timerInfo.AllCount--;
                timerInfo.Callback?.Invoke();
            }

            //如果取消了，则会抛出异常
            try
            {
                for (int i = 0; i < timerInfo.AllCount; i++)
                {
                    await UniTask.Delay(timerInfo.InviteTime, cancellationToken: timerInfo.Cts.Token);
                    timerInfo.Callback?.Invoke();
                };
            }
            catch (OperationCanceledException)
            {
                GameGod.Instance.Log(E_Log.Warring, "定时器主动取消", timerInfo.TimerName);
            }
            finally
            {
                //不管是时间到了还是主动取消，都会在这里进行回收处理
                _timerInfoDic.Remove(timerInfo.TimerName);
                TimerInfo.Recycle(timerInfo);
                GameGod.Instance.Log(E_Log.Framework, "定时器回收", timerInfo.TimerName);
            }
        }

        /// <summary>
        /// 获取定时器
        /// </summary>
        public TimerInfo GetTimerInfo(string timerName)
        {
            if (!_timerInfoDic.TryGetValue(timerName,out var timerInfo))
            {
                GameGod.Instance.Log(E_Log.Warring, "定时器不存在", timerName);
                return null;
            }
            return timerInfo;
        }

        /// <summary>
        /// 回收定时器
        /// </summary>
        public void RemoveTimer(string timerName)
        {
            if (_timerInfoDic.ContainsKey(timerName))
            {
                _timerInfoDic[timerName].Cts.Cancel();
            }
        }

        public override void OnUpdate() { }

        public override void OnDispose() 
        {
            var list = _timerInfoDic.Keys.ToList();
            foreach (var item in list)
            {
                RemoveTimer(item);
            }
            _timerInfoDic.Clear();
        }
    }
}
