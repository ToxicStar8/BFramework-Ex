using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Framework
{
    public class TaskInfo
    {
        /// <summary>
        /// 需要执行的操作
        /// </summary>
        public Action<TaskInfo> Task;

        public UniTaskCompletionSource Utcs;
        public UniTask WaitComplete() => Utcs.Task;

        /// <summary>
        /// Action中结束时必须执行这个操作
        /// </summary>
        public void OnComplete() => Utcs.TrySetResult();

        public static TaskInfo Create(Action<TaskInfo> task)
        {
            var taskInfo = GameGod.Instance.PoolManager.CreateClassObj<TaskInfo>();
            taskInfo.Task = task;
            taskInfo.Utcs = new UniTaskCompletionSource();
            return taskInfo;
        }
        public static void Recycle(TaskInfo taskInfo)
        {
            taskInfo.Task = null;
            taskInfo.Utcs = null;
            GameGod.Instance.PoolManager.RecycleClassObj(taskInfo);
        }
    }

    /// <summary>
    /// 任务队列管理器
    /// </summary>
    public partial class TaskManager : ManagerBase
    {
        private bool _isRunning;
        public Queue<TaskInfo> TaskQueue { get; private set; }

        public override void OnInit()
        {
            TaskQueue = new();
        }

        public void AddTask(Action<TaskInfo> task)
        {
            var taskInfo = TaskInfo.Create(task);
            TaskQueue.Enqueue(taskInfo);
        }

        public override async void OnUpdate()
        {
            if (!_isRunning)
            {
                if (TaskQueue.TryDequeue(out var taskInfo))
                {
                    _isRunning = true;

                    taskInfo.Task(taskInfo);
                    await taskInfo.WaitComplete();
                    TaskInfo.Recycle(taskInfo);

                    _isRunning = false;
                }
            }
        }

        public override void OnDispose()
        {
            TaskQueue.Clear();
            TaskQueue = null;
        }
    }
}