using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qtee.Contracts.Processors;

namespace Qtee.Processors.QCounter
{
    public sealed class CounterTaskResult : IQTask, IQTaskResult<decimal>
    {
        public CounterTaskResult()
        {
        }

        public void SetValue(decimal value)
        {
            this.BaseValue = value;
        }

        private decimal BaseValue { get; set; }

        public decimal GetResult()
        {
            return BaseValue;
        }

        public void Run()
        {
            BaseValue = BaseValue * BaseValue;
        }
    }

    public sealed class BasicCounter : IQProcessorSwicher, IQProcessorData<IQTaskResult<decimal>, decimal>, IDisposable
    {
        public BasicCounter()
        {
            TasksQueue = new ConcurrentQueue<IQTaskResult<decimal>>();
            TasksResultStack = new ConcurrentQueue<decimal>();
            ProcessorCancellationTokenSource = new CancellationTokenSource();
            TaskProcessor = null;
        }

        private void TaskProcessRun(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (ProcessorState == QProcessorState.Started || ProcessorState == QProcessorState.Wainting)
                    ProcessorState = QProcessorState.Processing;

                while (TasksQueue.IsEmpty == false)
                    if (TasksQueue.TryDequeue(out var result))
                    {
                        //run if needs
                        if (result is IQTask resultProcess)
                            resultProcess.Run();
                        //get result
                        TasksResultStack.Enqueue(result.GetResult());
                    }


                ProcessorState = QProcessorState.Wainting;
            }
        }

        private ConcurrentQueue<IQTaskResult<decimal>> TasksQueue { get; set; }
        private ConcurrentQueue<decimal> TasksResultStack { get; set; }
        private CancellationTokenSource ProcessorCancellationTokenSource { get; set; }

        private Task TaskProcessor { get; set; }
        private Object _taskProcessorLock = new Object();

        private QProcessorState ProcessorState { get; set; }

        public void Add(IQTaskResult<decimal> item)
        {
            TasksQueue.Enqueue(item);
        }

        public void AddList(IList<IQTaskResult<decimal>> collection)
        {
            foreach (var item in collection)
                TasksQueue.Enqueue(item);
        }

        public decimal GetLastOrNull()
        {
            decimal result = 0m;
            while (true)
                if (TasksResultStack.IsEmpty || TasksResultStack.TryDequeue(out result))
                    return result;
        }

        public IList<decimal> ToList()
        {
            return TasksResultStack.ToArray().ToList();
        }

        #region IQProcessorSwicher:

        /// <summary>
        /// Get current processor state
        /// </summary>
        /// <returns></returns>
        QProcessorState IQProcessorSwicher.GetState()
        {
            return ProcessorState;
        }

        public async Task<bool> WaitForState(QProcessorState state)
        {
            while (!this.ProcessorCancellationTokenSource.Token.IsCancellationRequested)
                if (this.ProcessorState == state)
                {
                    return true;
                }

            return false;
        }

        /// <summary>
        /// Starts processing, sets cancellation token, changes status, runs processing methond
        /// </summary>
        void IQProcessorSwicher.StartProcessing()
        {
            lock (_taskProcessorLock)
            {
                if (TaskProcessor == null || (TaskProcessor.Status != TaskStatus.Running
                                              && TaskProcessor.Status != TaskStatus.WaitingToRun
                                              && TaskProcessor.Status != TaskStatus.WaitingForActivation))
                {
                    if (ProcessorState != QProcessorState.Started)
                        ProcessorState = QProcessorState.Started;
                    ProcessorCancellationTokenSource = new CancellationTokenSource();
                    TaskProcessor = Task.Run(() => TaskProcessRun(ProcessorCancellationTokenSource.Token));
                }
            }
        }

        /// <summary>
        /// Stops processing, sets cancellation token, changes status
        /// </summary>
        void IQProcessorSwicher.StopProcessing()
        {
            lock (_taskProcessorLock)
            {
                if (ProcessorCancellationTokenSource.IsCancellationRequested)
                    return;
                ProcessorCancellationTokenSource.Cancel();
            }

            while (true)
                if (TaskProcessor.Status != TaskStatus.Running)
                {
                    ProcessorState = QProcessorState.Finished;
                    break;
                }
        }

        #endregion

        public void Dispose()
        {
            (this as IQProcessorSwicher).StopProcessing();
        }
    }
}