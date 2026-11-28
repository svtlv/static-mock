using System;
using StaticMock.HookBuilders;
using StaticMock.Hooks;

namespace StaticMock.Mocks.Callback;

internal class CallbackMock(IHookBuilder hookBuilder, IHookManager hookManager) : ICallbackMock
{
    public IReturnable Callback(Action callback)
    {
        var hookMethod = hookBuilder.CreateCallbackHook(callback);

        return hookManager.ApplyHook(hookMethod);
    }

    public IReturnable Callback<TArg>(Action<TArg> callback)
    {
        var hookMethod = hookBuilder.CreateCallbackHook(callback);

        return hookManager.ApplyHook(hookMethod);
    }

    public IReturnable Callback<TArg1, TArg2>(Action<TArg1, TArg2> callback)
    {
        var hookMethod = hookBuilder.CreateCallbackHook(callback);

        return hookManager.ApplyHook(hookMethod);
    }

    public IReturnable Callback<TArg1, TArg2, TArg3>(Action<TArg1, TArg2, TArg3> callback)
    {
        var hookMethod = hookBuilder.CreateCallbackHook(callback);

        return hookManager.ApplyHook(hookMethod);
    }

    public IReturnable Callback<TArg1, TArg2, TArg3, TArg4>(Action<TArg1, TArg2, TArg3, TArg4> callback)
    {
        var hookMethod = hookBuilder.CreateCallbackHook(callback);

        return hookManager.ApplyHook(hookMethod);
    }

    public IReturnable Callback<TArg1, TArg2, TArg3, TArg4, TArg5>(Action<TArg1, TArg2, TArg3, TArg4, TArg5> callback)
    {
        var hookMethod = hookBuilder.CreateCallbackHook(callback);

        return hookManager.ApplyHook(hookMethod);
    }

    public IReturnable Callback<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> callback)
    {
        var hookMethod = hookBuilder.CreateCallbackHook(callback);

        return hookManager.ApplyHook(hookMethod);
    }

    public IReturnable Callback<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> callback)
    {
        var hookMethod = hookBuilder.CreateCallbackHook(callback);

        return hookManager.ApplyHook(hookMethod);
    }

    public IReturnable Callback<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> callback)
    {
        var hookMethod = hookBuilder.CreateCallbackHook(callback);

        return hookManager.ApplyHook(hookMethod);
    }

    public IReturnable Callback<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> callback)
    {
        var hookMethod = hookBuilder.CreateCallbackHook(callback);

        return hookManager.ApplyHook(hookMethod);
    }
}