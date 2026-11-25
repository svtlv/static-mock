using System;
using System.Threading.Tasks;
using StaticMock.HookBuilders;
using StaticMock.Hooks;
using StaticMock.Mocks.Returns;

namespace StaticMock.Mocks.Implementation.Sequential;

internal class AsyncFuncMock<TReturnValue>(IHookBuilder hookBuilder, IHookManager hookManager)
    : FuncMock<Task<TReturnValue>>(hookBuilder, hookManager), IAsyncFuncMock<TReturnValue>
{
    public IDisposable ReturnsAsync(TReturnValue value) =>
        new ReturnsMock<TReturnValue>(HookBuilder, HookManager).ReturnsAsync(value);
}