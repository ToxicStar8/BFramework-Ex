using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 顺序执行异步任务，同一时间只运行一个。
    /// </summary>
    public partial class TaskManager : ManagerBase
    {
        private LinkedList<TaskInfo> _taskQueue;
        private TaskInfo _currentTask;
        private bool _isRunning;
        private bool _isDisposed;

        public override void OnAwake()
        {
            _taskQueue = new LinkedList<TaskInfo>();
        }

        public UniTask Enqueue(Func<CancellationToken, UniTask> task) =>
            AddTask(task, false);

        public UniTask EnqueueNext(Func<CancellationToken, UniTask> task) =>
            AddTask(task, true);

        // 只请求取消，当前任务真正返回后才会执行下一项。
        public bool CancelCurrent()
        {
            if (_currentTask == null)
            {
                return false;
            }

            _currentTask.Cancellation.Cancel();
            return true;
        }

        // 清空等待队列，不影响当前任务。
        public int ClearQueue()
        {
            var tasks = _taskQueue;
            var count = tasks.Count;
            _taskQueue = new LinkedList<TaskInfo>();

            foreach (var task in tasks)
            {
                task.Cancellation.Cancel();
                task.Completion.TrySetCanceled(task.Cancellation.Token);
            }

            return count;
        }

        public override void OnUpdate()
        {
        }

        public override void OnDispose()
        {
            _isDisposed = true;
            ClearQueue();
            CancelCurrent();
        }

        private UniTask AddTask(Func<CancellationToken, UniTask> execute, bool runNext)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(TaskManager));
            }

            var task = new TaskInfo(execute);
            var completion = task.Completion.Task;

            if (runNext)
            {
                _taskQueue.AddFirst(task);
            }
            else
            {
                _taskQueue.AddLast(task);
            }

            RunQueue().Forget();
            return completion;
        }

        private async UniTask RunQueue()
        {
            if (_isRunning)
            {
                return;
            }

            _isRunning = true;

            while (!_isDisposed && _taskQueue.First != null)
            {
                var task = _taskQueue.First.Value;
                _taskQueue.RemoveFirst();
                _currentTask = task;

                Exception error = null;
                try
                {
                    await task.Execute(task.Cancellation.Token);
                }
                catch (Exception exception)
                {
                    error = exception;
                }

                // 任务内部可以切线程；队列状态统一回主线程处理。
                await UniTask.SwitchToMainThread();
                CompleteTask(task, error);
            }

            _isRunning = false;
        }

        private void CompleteTask(TaskInfo task, Exception error)
        {
            _currentTask = null;
            var cancellationToken = task.Cancellation.Token;

            if (error is OperationCanceledException || cancellationToken.IsCancellationRequested)
            {
                task.Completion.TrySetCanceled(cancellationToken);
            }
            else if (error != null)
            {
                Debug.LogException(error);
                task.Completion.TrySetException(error);
            }
            else
            {
                task.Completion.TrySetResult();
            }

        }

        private sealed class TaskInfo
        {
            public readonly Func<CancellationToken, UniTask> Execute;
            public readonly CancellationTokenSource Cancellation = new();
            public readonly UniTaskCompletionSource Completion = new();

            public TaskInfo(Func<CancellationToken, UniTask> execute) => Execute = execute;
        }
    }
}
