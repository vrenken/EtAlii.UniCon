#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask.Interfaces
{
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityMvvmToolkit.Core.Interfaces;

    public interface IAsyncCommand : IBaseAsyncCommand, ICommand
    {
        UniTask ExecuteAsync(CancellationToken cancellationToken = default);
    }
}

#endif