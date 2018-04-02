using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qtee.Contracts.Processors
{
    [Flags]
    public enum QProcessorState
    {
        NotSet = 0x0,
        Started = 0x1,
        Processing = 0x2,
        Wainting = 0x4,
        Finished = 0x8
    }

    public enum QTaskState
    {
        NotSet,
    }

    public interface IQProcessorSwicher
    {
        void StartProcessing();
        void StopProcessing();
        QProcessorState GetState();
        Task<bool> WaitForState(QProcessorState state);
    }

    public interface IQProcessorData<TTaskType, TResultType> where TTaskType : IQTaskResult<TResultType>
    {
        void Add(TTaskType item);
        void AddList(IList<TTaskType> collection);
        TResultType GetLastOrNull();
        IList<TResultType> ToList();
    }

    public interface IQTask
    {
        void Run();
    }

    public interface IQTaskResult<out T>
    {
        T GetResult();
    }
}