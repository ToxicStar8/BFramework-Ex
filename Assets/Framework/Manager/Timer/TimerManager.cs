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
        public Action ExecCallback;         //执行回调
        public Action<bool> EndCallback;    //结束回调
        public int AllCount;                //执行次数
        public int InviteTime;              //执行间隔时间，毫秒
        public bool IsExecImmed;            //是否立即执行
        public CancellationTokenSource Cts; //取消用唯一Key
        //定时器管理器里赋值
        public string TimerName;            //定时器名

        public static TimerInfo Create(int allCount, int inviteTime, bool isExecImmed, Action execCallback = null, Action<bool> endCallback = null)
        {
            var timerInfo = GameManager.Instance.PoolManager.CreateClassObj<TimerInfo>();
            timerInfo.AllCount = allCount;
            timerInfo.InviteTime = inviteTime;
            timerInfo.IsExecImmed = isExecImmed;
            timerInfo.ExecCallback = execCallback;
            timerInfo.EndCallback = endCallback;
            timerInfo.Cts = new CancellationTokenSource();
            return timerInfo;
        }

        public static void Recycle(TimerInfo timerInfo)
        {
            timerInfo.Cts?.Dispose();
            timerInfo.Cts = null;
            timerInfo.ExecCallback = null;
            timerInfo.EndCallback = null;
            GameManager.Instance.PoolManager.RecycleClassObj(timerInfo);
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

        public override void OnAwake()
        {
            _timerInfoDic = new Dictionary<string, TimerInfo>();
        }

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
        public void AddTimer(string timerName, TimerInfo timerInfo)
        {
            if (timerInfo.AllCount <= 0)
            {
                GameManager.Instance.Log(E_Log.Error, "计时器的执行次数永远不能小于等于0！");
                return;
            }

            GameManager.Instance.Log(E_Log.Framework, "定时器添加", timerName);
            //记录名字
            timerInfo.TimerName = timerName;
            //添加到计时器的字典里
            _timerInfoDic.Add(timerName, timerInfo);
            ExecTimer(timerInfo).Forget();
        }

        /// <summary>
        /// 执行定时器
        /// </summary>
        private async UniTask ExecTimer(TimerInfo timerInfo)
        {
            try
            {
                //判断是否马上执行
                if (timerInfo.IsExecImmed)
                {
                    timerInfo.AllCount--;
                    timerInfo.ExecCallback.Invoke();
                }

                //初始化一个时间用来计算时间偏移
                long offsetTime = 0;
                for (int i = 0; i < timerInfo.AllCount; i++)
                {
                    //计算偏差值，初始化时值相同偏差为0
                    long startTime = TimeUtil.GetNowTimeMilliseconds();
                    //下次延迟时间 = 原来延迟的时间 - 上次偏移的时间        //例：inviteTime = 1000 - 1 = 999
                    int inviteTime = (int)(timerInfo.InviteTime - offsetTime);

                    // 确保延迟时间不为负数
                    if (inviteTime <= 0) inviteTime = 1;

                    await UniTask.Delay(inviteTime, cancellationToken: timerInfo.Cts.Token);

                    // 执行回调
                    timerInfo.ExecCallback.Invoke();

                    //新的偏移时间 = 现在时间 - 延迟前的时间 - 间隔时间     //例：offsetTime = 2001 - 1000 - 1000 = 1
                    offsetTime = TimeUtil.GetNowTimeMilliseconds() - startTime - inviteTime;
                }
                ;
            }
            catch (OperationCanceledException)
            {
                GameManager.Instance.Log(E_Log.Warning, "定时器主动取消", timerInfo.TimerName);
            }
            catch (Exception ex)
            {
                GameManager.Instance.Log(E_Log.Error, "定时器遇到意料之外的错误", ex.ToString());
            }
            finally
            {
                //不管是时间到了还是主动取消，都会在这里进行回收处理
                if (_timerInfoDic is not null)
                {
                    timerInfo.EndCallback?.Invoke(timerInfo.Cts.IsCancellationRequested);
                    _timerInfoDic.Remove(timerInfo.TimerName);
                    TimerInfo.Recycle(timerInfo);
                }
                GameManager.Instance.Log(E_Log.Framework, "定时器回收", timerInfo.TimerName);
            }
        }

        /// <summary>
        /// 获取定时器
        /// </summary>
        public TimerInfo GetTimerInfo(string timerName)
        {
            if (!_timerInfoDic.TryGetValue(timerName, out var timerInfo))
            {
                GameManager.Instance.Log(E_Log.Warning, "定时器不存在", timerName);
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
        
        public void RemoveTimer(TimerInfo timerInfo)
        {
            if (_timerInfoDic.ContainsKey(timerInfo.TimerName))
            {
                _timerInfoDic[timerInfo.TimerName].Cts.Cancel();
            }
        }

        public override void OnUpdate() { }

        public override void OnDispose()
        {
            if (_timerInfoDic is not null)
            {
                var list = _timerInfoDic.Keys.ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    RemoveTimer(list[i]);
                }
                _timerInfoDic.Clear();
                _timerInfoDic = null;
            }
        }
    }
}
