using System;
using StaticMock.HookBuilders;
using StaticMock.Hooks;

namespace StaticMock.Mocks.Returns;

internal class ReturnsMock<TValue>(IHookBuilder hookBuilder, IHookManager hookManager) : IReturnsMock<TValue>
{
    public IReturnable Returns(TValue value)
    {
        var hook = hookBuilder.CreateReturnHook(value);

        return hookManager.ApplyHook(hook);
    }

    public IReturnable Returns(Func<TValue> getValue)
    {
        var hook = hookBuilder.CreateReturnHook(getValue);

        return hookManager.ApplyHook(hook);
    }

    public IReturnable Returns<TArg>(Func<TArg, TValue> getValue)
    {
        var hook = hookBuilder.CreateReturnHook(getValue);

        return hookManager.ApplyHook(hook);
    }

    public IReturnable Returns<TArg1, TArg2>(Func<TArg1, TArg2, TValue> getValue)
    {
        var hook = hookBuilder.CreateReturnHook(getValue);

        return hookManager.ApplyHook(hook);
    }

    public IReturnable Returns<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, TValue> getValue)
    {
        var hook = hookBuilder.CreateReturnHook(getValue);

        return hookManager.ApplyHook(hook);
    }

    public IReturnable Returns<TArg1, TArg2, TArg3, TArg4>(Func<TArg1, TArg2, TArg3, TArg4, TValue> getValue)
    {
        var hook = hookBuilder.CreateReturnHook(getValue);

        return hookManager.ApplyHook(hook);
    }

    public IReturnable Returns<TArg1, TArg2, TArg3, TArg4, TArg5>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TValue> getValue)
    {
        var hook = hookBuilder.CreateReturnHook(getValue);

        return hookManager.ApplyHook(hook);
    }

    public IReturnable Returns<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TValue> getValue)
    {
        var hook = hookBuilder.CreateReturnHook(getValue);

        return hookManager.ApplyHook(hook);
    }

    public IReturnable Returns<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TValue> getValue)
    {
        var hook = hookBuilder.CreateReturnHook(getValue);

        return hookManager.ApplyHook(hook);
    }

    public IReturnable Returns<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TValue> getValue)
    {
        var hook = hookBuilder.CreateReturnHook(getValue);

        return hookManager.ApplyHook(hook);
    }

    public IReturnable Returns<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TValue> getValue)
    {
        var hook = hookBuilder.CreateReturnHook(getValue);

        return hookManager.ApplyHook(hook);
    }

    public IReturnable ReturnsAsync(TValue value)
    {
        var hook = hookBuilder.CreateReturnAsyncHook(value);

        return hookManager.ApplyHook(hook);
    }
}