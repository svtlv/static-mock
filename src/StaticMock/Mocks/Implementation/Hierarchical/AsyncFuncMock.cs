using System;
using System.Threading.Tasks;
using StaticMock.HookBuilders;
using StaticMock.Hooks;
using StaticMock.Mocks.Returns;

namespace StaticMock.Mocks.Implementation.Hierarchical;

internal class AsyncFuncMock<TReturnValue>(
    IHookBuilder hookBuilder,
    IHookManager hookManager,
    Action action)
    : FuncMock<Task<TReturnValue>>(hookBuilder, hookManager, action), IAsyncFuncMock<TReturnValue>
{
    public IDisposable ReturnsAsync(TReturnValue value)
    {
        var returnService = new ReturnsMock<TReturnValue>(HookBuilder, HookManager);
        using (returnService.ReturnsAsync(value))
        {
            ValidationAction();
        }

        return new Disposable();
    }
}